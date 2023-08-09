// Copyright (c) Omar Boukli-Hacene. All rights reserved.
// Distributed under an MIT-style license that can be
// found in the LICENSE file.

// SPDX-License-Identifier: MIT

using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Aktabook.Application.Messages.Commands;
using Aktabook.Application.Services;
using Aktabook.Bus.Handlers;
using Aktabook.Connectors.OpenLibrary;
using Aktabook.Connectors.OpenLibrary.Models;
using Aktabook.Data;
using Aktabook.Domain.Models;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging.Abstractions;
using NServiceBus.Testing;
using NSubstitute;
using Xunit;

namespace Aktabook.Bus.UnitTest;

public class BookInfoRequestHandlerUnitTest
{
    private const string ServiceName = "SUT";
    private readonly ActivitySource _activitySource = new(ServiceName);
    private readonly IBookInfoRequester _bookInfoRequestServiceMock = Substitute.For<IBookInfoRequester>();
    private readonly DbSet<Book> _bookSetMock = Substitute.For<DbSet<Book>>();
    private readonly IOpenLibraryClient _openLibraryClientMock = Substitute.For<IOpenLibraryClient>();
    private readonly RequesterServiceDbContext _requesterServiceDbContextMock = Substitute.For<RequesterServiceDbContext>();

    [Fact]
    public async Task GivenHandle_WhenProcessBookInfoRequest_ThenPublishThreeMessages()
    {
        _bookInfoRequestServiceMock.PlaceRequest(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new Guid("00000000-1111-0000-0000-000000000001")));

        _bookInfoRequestServiceMock.ChangeRequestStatus(Arg.Any<Guid>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(true));

        _openLibraryClientMock.GetBookByIsbnAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Work?>(new Work()));

        _bookSetMock.AddAsync(Arg.Any<Book>(), Arg.Any<CancellationToken>())
            .Returns((EntityEntry<Book>)default!);
        _requesterServiceDbContextMock.Books.Returns(_bookSetMock);
        _requesterServiceDbContextMock.SaveChangesAsync(Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(1));

        NullLogger<BookInfoRequestHandler> logger =
            NullLogger<BookInfoRequestHandler>.Instance;

        BookInfoRequestHandler handler = new(_bookInfoRequestServiceMock,
            _openLibraryClientMock, _requesterServiceDbContextMock, _activitySource, logger);

        TestableMessageHandlerContext context = new();

        await handler
            .Handle(new ProcessBookInfoRequest(Guid.Empty, "Dummy ISBN"), context)
            .ConfigureAwait(false);

        context.PublishedMessages.Should().HaveCount(3);
        context.RepliedMessages.Should().BeEmpty();
    }

    [Fact]
    public async Task GivenHandle_WhenGetBookByIsbnAsyncReturnsWork_ThenSaveToDbContext()
    {
        _bookInfoRequestServiceMock.PlaceRequest(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new Guid("00000000-1111-0000-0000-000000000001")));

        _bookInfoRequestServiceMock.ChangeRequestStatus(Arg.Any<Guid>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(true));

        _openLibraryClientMock.GetBookByIsbnAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Work?>(new Work()));

#pragma warning disable CA2012
        _bookSetMock.AddAsync(Arg.Any<Book>(), Arg.Any<CancellationToken>()).ReturnsForAnyArgs(x => ValueTask.FromResult<EntityEntry<Book>>(default!));
#pragma warning restore

        _requesterServiceDbContextMock.Books.Returns(_bookSetMock);
        _requesterServiceDbContextMock.SaveChangesAsync(Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(1));

        NullLogger<BookInfoRequestHandler> logger =
            NullLogger<BookInfoRequestHandler>.Instance;

        BookInfoRequestHandler handler = new(_bookInfoRequestServiceMock,
            _openLibraryClientMock, _requesterServiceDbContextMock, _activitySource, logger);

        TestableMessageHandlerContext context = new();

        await handler
            .Handle(new ProcessBookInfoRequest(Guid.Empty, "Dummy ISBN"), context)
            .ConfigureAwait(false);

        await _bookSetMock.Received(1).AddAsync(Arg.Any<Book>(), Arg.Any<CancellationToken>()).ConfigureAwait(false);
        await _requesterServiceDbContextMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>()).ConfigureAwait(false);
    }

    [Fact]
    public async Task GivenHandle_WhenGetBookByIsbnAsyncReturnsNull_ThenPublishThreeMessages()
    {
        _bookInfoRequestServiceMock.PlaceRequest(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new Guid("00000000-1111-0000-0000-000000000001")));

        _bookInfoRequestServiceMock.ChangeRequestStatus(Arg.Any<Guid>(), Arg.Any<string>(),
                Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(true));

        _openLibraryClientMock.GetBookByIsbnAsync(Arg.Any<string>(),
                Arg.Any<CancellationToken>())
            .Returns(Task.FromResult((Work?)null));
#pragma warning disable CA2012
        _bookSetMock.AddAsync(Arg.Any<Book>(), Arg.Any<CancellationToken>())
            .ReturnsForAnyArgs(x => ValueTask.FromResult<EntityEntry<Book>>(default!));
#pragma warning restore

        _requesterServiceDbContextMock.Books.Returns(_bookSetMock);
        _requesterServiceDbContextMock.SaveChangesAsync(Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(0));

        NullLogger<BookInfoRequestHandler> logger =
            NullLogger<BookInfoRequestHandler>.Instance;

        BookInfoRequestHandler handler = new(_bookInfoRequestServiceMock,
            _openLibraryClientMock, _requesterServiceDbContextMock, _activitySource, logger);

        TestableMessageHandlerContext context = new();

        await handler
            .Handle(new ProcessBookInfoRequest(Guid.Empty, "Dummy ISBN"), context)
            .ConfigureAwait(false);

        context.PublishedMessages.Should().HaveCount(3);
        context.RepliedMessages.Should().BeEmpty();
    }

    [Fact]
    public async Task GivenHandle_WhenGetBookByIsbnAsyncReturnsNull_ThenDoNotUseDbContext()
    {
        _bookInfoRequestServiceMock.PlaceRequest(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new Guid("00000000-1111-0000-0000-000000000001")));

        _bookInfoRequestServiceMock.ChangeRequestStatus(Arg.Any<Guid>(), Arg.Any<string>(),
                Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(true));

        _openLibraryClientMock.GetBookByIsbnAsync(Arg.Any<string>(),
                Arg.Any<CancellationToken>())
            .Returns(Task.FromResult((Work?)null));

#pragma warning disable CA2012
        _bookSetMock.AddAsync(Arg.Any<Book>(), Arg.Any<CancellationToken>())
            .ReturnsForAnyArgs(x => ValueTask.FromResult<EntityEntry<Book>>(default!));
#pragma warning restore

        _requesterServiceDbContextMock.Books.Returns(_bookSetMock);
        _requesterServiceDbContextMock.SaveChangesAsync(Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(0));

        NullLogger<BookInfoRequestHandler> logger =
            NullLogger<BookInfoRequestHandler>.Instance;

        BookInfoRequestHandler handler = new(_bookInfoRequestServiceMock,
            _openLibraryClientMock, _requesterServiceDbContextMock, _activitySource, logger);

        TestableMessageHandlerContext context = new();

        await handler
            .Handle(new ProcessBookInfoRequest(Guid.Empty, "Dummy ISBN"), context)
            .ConfigureAwait(false);

        var _ = _bookSetMock.DidNotReceive().AddAsync(Arg.Any<Book>(), Arg.Any<CancellationToken>());
        await _requesterServiceDbContextMock.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>()).ConfigureAwait(false);
    }
}
