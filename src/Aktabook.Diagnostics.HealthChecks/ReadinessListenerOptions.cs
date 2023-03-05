// Copyright (c) Omar Boukli-Hacene. All rights reserved.
// Distributed under an MIT-style license that can be
// found in the LICENSE file.

// SPDX-License-Identifier: MIT

namespace Aktabook.Diagnostics.HealthChecks;

public class ReadinessListenerOptions : HealthCheckTcpServiceOptions
{
    public ReadinessListenerOptions()
    {
        Port = 23514;
    }
}
