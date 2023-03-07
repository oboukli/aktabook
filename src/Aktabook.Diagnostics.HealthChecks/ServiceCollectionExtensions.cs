// Copyright (c) Omar Boukli-Hacene. All rights reserved.
// Distributed under an MIT-style license that can be
// found in the LICENSE file.

// SPDX-License-Identifier: MIT

using Microsoft.Extensions.DependencyInjection;

namespace Aktabook.Diagnostics.HealthChecks;

public static class ServiceCollectionExtensions
{
    public static void AddHealthCheckTcpEndpoint(this IServiceCollection services)
    {
        services.AddSingleton<IReadinessListener, ReadinessListener>();
        services.AddSingleton<ILivenessListener, LivenessListener>();
        services.AddHostedService<HealthCheckBackgroundService>();
    }
}
