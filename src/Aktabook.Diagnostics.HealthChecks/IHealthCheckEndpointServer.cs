// Copyright (c) 2022 Omar Boukli-Hacene. All rights reserved.
// Distributed under an MIT-style license that can be
// found in the LICENSE file.

// SPDX-License-Identifier: MIT

namespace Aktabook.Diagnostics.HealthChecks;

public interface IHealthCheckEndpointServer
{
    void SetStoppingToken(CancellationToken stoppingToken);

    Task StartAsync();

    void Stop();
}
