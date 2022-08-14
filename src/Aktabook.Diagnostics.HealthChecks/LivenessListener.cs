// Copyright (c) 2022 Omar Boukli-Hacene. All rights reserved.
// Distributed under an MIT-style license that can be
// found in the LICENSE file.

// SPDX-License-Identifier: MIT

using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Aktabook.Diagnostics.HealthChecks;

internal sealed class LivenessListener : HealthCheckEndpointServer,
    ILivenessListener
{
    public LivenessListener(HealthCheckService healthCheckService,
        ILogger<LivenessListener> logger,
        IOptions<LivenessListenerOptions> options)
        : base(healthCheckService, logger, options)
    {
    }
}