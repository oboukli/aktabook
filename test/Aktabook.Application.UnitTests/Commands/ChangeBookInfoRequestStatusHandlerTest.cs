// Copyright (c) 2022 Omar Boukli-Hacene. All rights reserved.
// Distributed under an MIT-style license that can be
// found in the LICENSE file.

// SPDX-License-Identifier: MIT

using System;
using System.Threading;
using System.Threading.Tasks;
using Aktabook.Application.Commands;
using Aktabook.Application.Services;
using MediatR;
using Moq;
using Xunit;

namespace Aktabook.Application.UnitTests.Commands;

public class ChangeBookInfoRequestStatusHandlerTest
{
    private readonly Mock<IBookInfoRequestService> _bookInfoRequestServiceMock =
        new(MockBehavior.Strict);

    [Fact]
    public async Task GivenHandle_WhenCommand_ThenChangeBookInfoRequestStatus()
    {
        _bookInfoRequestServiceMock.Setup(x =>
            x.ChangeRequestStatus(It.IsAny<Guid>(), It.IsAny<string>(),
                It.IsAny<CancellationToken>())).ReturnsAsync(true);

        ChangeBookInfoRequestStatus changeBookInfoRequestStatus =
            new(new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"), "Dummy");
        ChangeBookInfoRequestStatusHandler handler =
            new(_bookInfoRequestServiceMock.Object);

        Unit unit = await handler.Handle(changeBookInfoRequestStatus,
            CancellationToken.None);

        _bookInfoRequestServiceMock.Verify(
            x => x.ChangeRequestStatus(
                new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"), "Dummy",
                CancellationToken.None), Times.Once);
    }
}