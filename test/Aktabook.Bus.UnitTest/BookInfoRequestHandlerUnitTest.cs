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
using FluentAssertions;
using Moq;
using NServiceBus.Testing;
using Xunit;

namespace Aktabook.Bus.UnitTest;

public class BookInfoRequestHandlerUnitTest
{
    private const string ServiceName = "SUT";
    private readonly ActivitySource _activitySource = new(ServiceName);
    private readonly Mock<IBookInfoRequestService> _bookInfoRequestServiceMock;
    private readonly Mock<IOpenLibraryClient> _openLibraryClientMock;

    public BookInfoRequestHandlerUnitTest()
    {
        _bookInfoRequestServiceMock = new Mock<IBookInfoRequestService>(MockBehavior.Strict);
        _openLibraryClientMock = new Mock<IOpenLibraryClient>(MockBehavior.Strict);
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

        BookInfoRequestHandler handler = new(_bookInfoRequestServiceMock.Object,
            _openLibraryClientMock.Object, _activitySource);

        TestableMessageHandlerContext context = new();

        await handler
            .Handle(new ProcessBookInfoRequest(Guid.Empty, "Dummy ISBN"), context)
            .ConfigureAwait(false);

        context.PublishedMessages.Should().HaveCount(3);
        context.RepliedMessages.Should().BeEmpty();
    }
}