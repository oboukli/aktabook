// Copyright (c) 2022 Omar Boukli-Hacene. All rights reserved.
// Distributed under an MIT-style license that can be
// found in the LICENSE file.

// SPDX-License-Identifier: MIT

using System;
using System.Threading;
using System.Threading.Tasks;
using Aktabook.Application.MessageHandlers;
using Aktabook.Application.Messages.Commands;
using Aktabook.Application.Services;
using FluentAssertions;
using Moq;
using NServiceBus;
using Xunit;

namespace Aktabook.Application.UnitTests.MessageHandlers;

public class PlaceBookInfoRequestHandlerTest
{
    private readonly Mock<IBookInfoRequestService> _bookInfoRequestServiceMock = new(MockBehavior.Strict);

    private readonly Mock<IEndpointInstance> _endpointInstanceMock = new(MockBehavior.Strict);

    [Fact]
    public async Task GivenHandle_WhenCommand_ThenBookInfoRequestId()
    {
        PlaceBookInfoRequest placeBookInfoRequest = new("Dummy ISBN");

        _bookInfoRequestServiceMock.Setup(x =>
                x.PlaceRequest(It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"));

        _endpointInstanceMock.Setup(x =>
                x.Send(It.IsAny<object>(), It.IsAny<SendOptions>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        PlaceBookInfoRequestHandler handler =
            new(_bookInfoRequestServiceMock.Object,
                _endpointInstanceMock.Object);

        Guid bookInfoRequestId = await handler.Handle(placeBookInfoRequest, CancellationToken.None)
            .ConfigureAwait(false);

        bookInfoRequestId.Should().Be(new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"));
    }
}
