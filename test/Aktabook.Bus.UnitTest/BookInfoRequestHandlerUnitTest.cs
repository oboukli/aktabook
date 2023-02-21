// Copyright (c) 2022 Omar Boukli-Hacene. All rights reserved.
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
using Moq;
using NServiceBus.Testing;
using Xunit;

namespace Aktabook.Bus.UnitTest;

public class BookInfoRequestHandlerUnitTest
{
    private const string ServiceName = "SUT";
    private readonly ActivitySource _activitySource = new(ServiceName);
    private readonly Mock<IBookInfoRequestService> _bookInfoRequestServiceMock;
    private readonly Mock<DbSet<Book>> _bookSetMock;
    private readonly Mock<IOpenLibraryClient> _openLibraryClientMock;
    private readonly Mock<RequesterServiceDbContext> _requesterServiceDbContextMock;

    public BookInfoRequestHandlerUnitTest()
    {
        _bookInfoRequestServiceMock = new Mock<IBookInfoRequestService>(MockBehavior.Strict);
        _bookSetMock = new Mock<DbSet<Book>>(MockBehavior.Strict);
        _openLibraryClientMock = new Mock<IOpenLibraryClient>(MockBehavior.Strict);
        _requesterServiceDbContextMock = new Mock<RequesterServiceDbContext>(MockBehavior.Strict);
    }

    [Fact]
    public async Task GivenHandle_WhenProcessBookInfoRequest_ThenPublishThreeMessages()
    {
        _bookInfoRequestServiceMock.Setup(x =>
                x.PlaceRequest(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new Guid("00000000-1111-0000-0000-000000000001"));

        _bookInfoRequestServiceMock.Setup(x =>
                x.ChangeRequestStatus(It.IsAny<Guid>(), It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => true);

        _openLibraryClientMock.Setup(x =>
                x.GetBookByIsbnAsync(It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Work());

        _bookSetMock.Setup(x => x.AddAsync(It.IsAny<Book>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => default!);
        _requesterServiceDbContextMock.Setup(x => x.Books).Returns(_bookSetMock.Object);
        _requesterServiceDbContextMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => 1);

        BookInfoRequestHandler handler = new(_bookInfoRequestServiceMock.Object,
            _openLibraryClientMock.Object, _requesterServiceDbContextMock.Object, _activitySource);

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
        _bookInfoRequestServiceMock.Setup(x =>
                x.PlaceRequest(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new Guid("00000000-1111-0000-0000-000000000001"));

        _bookInfoRequestServiceMock.Setup(x =>
                x.ChangeRequestStatus(It.IsAny<Guid>(), It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => true);

        _openLibraryClientMock.Setup(x =>
                x.GetBookByIsbnAsync(It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Work());

        _bookSetMock.Setup(x => x.AddAsync(It.IsAny<Book>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => default!);
        _requesterServiceDbContextMock.Setup(x => x.Books).Returns(_bookSetMock.Object);
        _requesterServiceDbContextMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => 1);

        BookInfoRequestHandler handler = new(_bookInfoRequestServiceMock.Object,
            _openLibraryClientMock.Object, _requesterServiceDbContextMock.Object, _activitySource);

        TestableMessageHandlerContext context = new();

        await handler
            .Handle(new ProcessBookInfoRequest(Guid.Empty, "Dummy ISBN"), context)
            .ConfigureAwait(false);

        _bookSetMock.Verify(x => x.AddAsync(It.IsAny<Book>(), It.IsAny<CancellationToken>()),
            Times.Once());
        _requesterServiceDbContextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once());
    }

    [Fact]
    public async Task GivenHandle_WhenGetBookByIsbnAsyncReturnsNull_ThenPublishThreeMessages()
    {
        _bookInfoRequestServiceMock.Setup(x =>
                x.PlaceRequest(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new Guid("00000000-1111-0000-0000-000000000001"));

        _bookInfoRequestServiceMock.Setup(x =>
                x.ChangeRequestStatus(It.IsAny<Guid>(), It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => true);

        _openLibraryClientMock.Setup(x =>
                x.GetBookByIsbnAsync(It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync((Work?)null);

        _bookSetMock.Setup(x => x.AddAsync(It.IsAny<Book>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => default!);
        _requesterServiceDbContextMock.Setup(x => x.Books).Returns(_bookSetMock.Object);
        _requesterServiceDbContextMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => 0);

        BookInfoRequestHandler handler = new(_bookInfoRequestServiceMock.Object,
            _openLibraryClientMock.Object, _requesterServiceDbContextMock.Object, _activitySource);

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
        _bookInfoRequestServiceMock.Setup(x =>
                x.PlaceRequest(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new Guid("00000000-1111-0000-0000-000000000001"));

        _bookInfoRequestServiceMock.Setup(x =>
                x.ChangeRequestStatus(It.IsAny<Guid>(), It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => true);

        _openLibraryClientMock.Setup(x =>
                x.GetBookByIsbnAsync(It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync((Work?)null);

        _bookSetMock.Setup(x => x.AddAsync(It.IsAny<Book>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => default!);
        _requesterServiceDbContextMock.Setup(x => x.Books).Returns(_bookSetMock.Object);
        _requesterServiceDbContextMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => 0);

        BookInfoRequestHandler handler = new(_bookInfoRequestServiceMock.Object,
            _openLibraryClientMock.Object, _requesterServiceDbContextMock.Object, _activitySource);

        TestableMessageHandlerContext context = new();

        await handler
            .Handle(new ProcessBookInfoRequest(Guid.Empty, "Dummy ISBN"), context)
            .ConfigureAwait(false);

        _bookSetMock.Verify(x => x.AddAsync(It.IsAny<Book>(), It.IsAny<CancellationToken>()),
            Times.Never);
        _requesterServiceDbContextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never());
    }
}
