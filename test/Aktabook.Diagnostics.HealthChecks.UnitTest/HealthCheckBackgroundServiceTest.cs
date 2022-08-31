// Copyright (c) 2022 Omar Boukli-Hacene. All rights reserved.
// Distributed under an MIT-style license that can be
// found in the LICENSE file.

// SPDX-License-Identifier: MIT

using Microsoft.Extensions.Logging.Abstractions;

namespace Aktabook.Diagnostics.HealthChecks.UnitTest;

public class HealthCheckBackgroundServiceTest
{
    [Fact]
    public void GivenStopAsync_WhenAwaited_ThenDoesNotThrow()
    {
        Mock<IReadinessListener> readinessListenerMock = new(MockBehavior.Strict);
        readinessListenerMock.Setup(x => x.SetStoppingToken(It.IsAny<CancellationToken>()));
        Mock<ILivenessListener> livenessListenerMock = new(MockBehavior.Strict);
        livenessListenerMock.Setup(x => x.SetStoppingToken(It.IsAny<CancellationToken>()));
        NullLogger<HealthCheckBackgroundService> logger = NullLogger<HealthCheckBackgroundService>.Instance;

        HealthCheckBackgroundService healthCheckBackgroundService =
            new(readinessListenerMock.Object, livenessListenerMock.Object, logger);

        healthCheckBackgroundService.Awaiting(x => x.StopAsync(CancellationToken.None))
            .Should().NotThrowAsync();
    }
}