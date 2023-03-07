// Copyright (c) Omar Boukli-Hacene. All rights reserved.
// Distributed under an MIT-style license that can be
// found in the LICENSE file.

// SPDX-License-Identifier: MIT

using System;
using System.Threading.Tasks;
using Aktabook.Application.Messages.Commands;
using Aktabook.Bus.IntegrationTest.Fixtures;
using FluentAssertions;
using NServiceBus;
using Xunit;

namespace Aktabook.Bus.IntegrationTest;

public class EndpointTest : IClassFixture<BusEndpointFixture>
{
    private readonly BusEndpointFixture _busEndpointFixture;

    public EndpointTest(BusEndpointFixture busEndpointFixture)
    {
        _busEndpointFixture = busEndpointFixture;
    }

    [Fact]
    public async Task GivenSend_WhenMessage_ThenMessageInQueue()
    {
        IEndpointInstance endpointInstance = await _busEndpointFixture
            .GetEndpointInstance().ConfigureAwait(false);

        Guid bookInfoRequestId = Guid.NewGuid();

        ProcessBookInfoRequest processBookInfoRequest = new(bookInfoRequestId, "Dummy ISBN");

        await endpointInstance.Awaiting(x => x
            .Send(processBookInfoRequest)).Should().NotThrowAsync().ConfigureAwait(false);
    }
}
