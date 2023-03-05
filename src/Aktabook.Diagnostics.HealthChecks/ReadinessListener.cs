// Copyright (c) Omar Boukli-Hacene. All rights reserved.
// Distributed under an MIT-style license that can be
// found in the LICENSE file.

// SPDX-License-Identifier: MIT

using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Aktabook.Diagnostics.HealthChecks;

#pragma warning disable CA1812
internal sealed class ReadinessListener : HealthCheckEndpointServer, IReadinessListener
#pragma warning restore CA1812
{
    public ReadinessListener(HealthCheckService healthCheckService,
        ILogger<ReadinessListener> logger,
        IOptions<ReadinessListenerOptions> options)
        : base(healthCheckService, logger, options)
    {
    }
}
