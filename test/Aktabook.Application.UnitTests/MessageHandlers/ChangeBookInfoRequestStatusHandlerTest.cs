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
using NSubstitute;
using Xunit;

namespace Aktabook.Application.UnitTests.MessageHandlers;

public class ChangeBookInfoRequestStatusHandlerTest
{
    private readonly IBookInfoRequester _bookInfoRequestServiceMock = Substitute.For<IBookInfoRequester>();

    [Fact]
    public async Task GivenHandle_WhenCommand_ThenChangeBookInfoRequestStatus()
    {
        _bookInfoRequestServiceMock.ChangeRequestStatus(Arg.Any<Guid>(), Arg.Any<string>(),
                Arg.Any<CancellationToken>()).Returns(Task.FromResult(true));

        ChangeBookInfoRequestStatus changeBookInfoRequestStatus =
            new(new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"), "Dummy");
        ChangeBookInfoRequestStatusHandler handler = new(_bookInfoRequestServiceMock);

        bool result = await handler.Handle(changeBookInfoRequestStatus, CancellationToken.None)
            .ConfigureAwait(true);

        await _bookInfoRequestServiceMock.Received(1).ChangeRequestStatus(
                new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"), "Dummy",
                CancellationToken.None).ConfigureAwait(true);

        result.Should().BeTrue();
    }
}
