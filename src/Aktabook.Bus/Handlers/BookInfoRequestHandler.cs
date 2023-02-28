// Copyright (c) 2022 Omar Boukli-Hacene. All rights reserved.
// Distributed under an MIT-style license that can be
// found in the LICENSE file.

// SPDX-License-Identifier: MIT

using System.Diagnostics;
using Aktabook.Application.Messages.Commands;
using Aktabook.Application.Messages.Events;
using Aktabook.Application.Services;
using Aktabook.Connectors.OpenLibrary;
using Aktabook.Connectors.OpenLibrary.Models;
using Aktabook.Data;
using Aktabook.Domain.Models;
using NServiceBus.Logging;

namespace Aktabook.Bus.Handlers;

public class BookInfoRequestHandler : IHandleMessages<ProcessBookInfoRequest>
{
    private static readonly ILog Log = LogManager.GetLogger<BookInfoRequestHandler>();
    private readonly ActivitySource _activitySource;

    private readonly IBookInfoRequestService _bookInfoRequestService;
    private readonly IOpenLibraryClient _openLibraryClient;
    private readonly RequesterServiceDbContext _requesterServiceDbContext;

    public BookInfoRequestHandler(
        IBookInfoRequestService bookInfoRequestService,
        IOpenLibraryClient openLibraryClient,
        RequesterServiceDbContext requesterServiceDbContext,
        ActivitySource activitySource)
    {
        _bookInfoRequestService = bookInfoRequestService;
        _openLibraryClient = openLibraryClient;
        _requesterServiceDbContext = requesterServiceDbContext;
        _activitySource = activitySource;
    }

    public async Task Handle(ProcessBookInfoRequest message, IMessageHandlerContext context)
    {
        using Activity? activity =
            _activitySource.StartActivity(nameof(ProcessBookInfoRequest), ActivityKind.Consumer);
        activity?.AddEvent(new ActivityEvent(nameof(ChangeRequestStatus)));
        activity?.SetTag(Constants.TelemetryTags.FunctionTagKey, Constants.TelemetryTags.FunctionTagValue);

        Log.Info($@"{nameof(ProcessBookInfoRequest)} received ""{message.BookInfoRequestId}""");

        await ChangeRequestStatus(message.BookInfoRequestId, BookInfoRequestStatus.InProgress)
            .ConfigureAwait(false);

        await context
            .Publish(new BookInfoRequestStatusChanged(message.BookInfoRequestId,
                BookInfoRequestStatus.InProgress))
            .ConfigureAwait(false);

        activity?.AddEvent(new ActivityEvent(nameof(IOpenLibraryClient.GetBookByIsbnAsync)));

        string requestStatus;
        Work? work = await _openLibraryClient.GetBookByIsbnAsync(message.Isbn, CancellationToken.None)
            .ConfigureAwait(false);
        if (work is { })
        {
            await CreateBookFromWork(work, CancellationToken.None).ConfigureAwait(false);
            requestStatus = BookInfoRequestStatus.Fulfilled;
        }
        else
        {
            requestStatus = BookInfoRequestStatus.Failed;
        }

        activity?.AddEvent(new ActivityEvent(nameof(ChangeRequestStatus)));

        await ChangeRequestStatus(message.BookInfoRequestId, requestStatus)
            .ConfigureAwait(false);

        await context.Publish(new BookInfoRequestProcessed(message.BookInfoRequestId)).ConfigureAwait(false);

        await context.Publish(new BookInfoRequestStatusChanged(message.BookInfoRequestId, requestStatus))
            .ConfigureAwait(false);
    }

    private async Task ChangeRequestStatus(Guid bookInfoRequestId, string status)
    {
        bool success = await _bookInfoRequestService
            .ChangeRequestStatus(bookInfoRequestId, status, CancellationToken.None)
            .ConfigureAwait(false);

        if (success is false)
        {
            Log.Error($@"{nameof(ProcessBookInfoRequest)} change status failed ""{bookInfoRequestId}""");
        }
    }

    private async Task CreateBookFromWork(Work work, CancellationToken cancellationToken)
    {
        Book book = WorkToBook(work);
        await _requesterServiceDbContext.Books.AddAsync(book, cancellationToken)
            .ConfigureAwait(false);
        await _requesterServiceDbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    private static Book WorkToBook(Work work)
    {
        string isbn10 = work.Isbn10.FirstOrDefault() ?? string.Empty;
        return new Book
        {
            Isbn = isbn10,
            Title = work.Title
        };
    }
}
