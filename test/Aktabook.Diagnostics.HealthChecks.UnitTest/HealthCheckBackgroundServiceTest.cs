// Copyright (c) Omar Boukli-Hacene. All rights reserved.
// Distributed under an MIT-style license that can be
// found in the LICENSE file.

// SPDX-License-Identifier: MIT

using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;

namespace Aktabook.Diagnostics.HealthChecks.UnitTest;

public class HealthCheckBackgroundServiceTest
{
    [Fact]
    public void GivenStopAsync_WhenAwaited_ThenDoesNotThrow()
    {
        IReadinessListener readinessListenerMock = Substitute.For<IReadinessListener>();
        readinessListenerMock.SetStoppingToken(Arg.Any<CancellationToken>());
        ILivenessListener livenessListenerMock = Substitute.For<ILivenessListener>();
        livenessListenerMock.SetStoppingToken(Arg.Any<CancellationToken>());
        NullLogger<HealthCheckBackgroundService> logger = NullLogger<HealthCheckBackgroundService>.Instance;

        HealthCheckBackgroundService healthCheckBackgroundService = new(readinessListenerMock, livenessListenerMock, logger);

        healthCheckBackgroundService.Awaiting(x => x.StopAsync(CancellationToken.None))
            .Should().NotThrowAsync();
    }
}
