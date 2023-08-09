// Copyright (c) Omar Boukli-Hacene. All rights reserved.
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
using NServiceBus;
using NSubstitute;
using Xunit;

namespace Aktabook.Application.UnitTests.MessageHandlers;

public class PlaceBookInfoRequestHandlerTest
{
    private readonly IBookInfoRequester _bookInfoRequestServiceMock = Substitute.For<IBookInfoRequester>();

    private readonly IEndpointInstance _endpointInstanceMock = Substitute.For<IEndpointInstance>();

    [Fact]
    public async Task GivenHandle_WhenCommand_ThenBookInfoRequestId()
    {
        PlaceBookInfoRequest placeBookInfoRequest = new("Dummy ISBN");

        _bookInfoRequestServiceMock.PlaceRequest(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd")));

        _endpointInstanceMock.Send(Arg.Any<object>(), Arg.Any<SendOptions>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        PlaceBookInfoRequestHandler handler = new(_bookInfoRequestServiceMock, _endpointInstanceMock);

        Guid bookInfoRequestId = await handler.Handle(placeBookInfoRequest, CancellationToken.None)
            .ConfigureAwait(false);

        bookInfoRequestId.Should().Be(new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"));
    }
}
