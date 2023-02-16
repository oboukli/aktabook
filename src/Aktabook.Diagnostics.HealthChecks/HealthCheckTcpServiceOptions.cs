// Copyright (c) 2022 Omar Boukli-Hacene. All rights reserved.
// Distributed under an MIT-style license that can be
// found in the LICENSE file.

// SPDX-License-Identifier: MIT

using System.Net;

namespace Aktabook.Diagnostics.HealthChecks;

public class HealthCheckTcpServiceOptions
{
    public string Name { get; set; } = string.Empty;

    public ISet<string> Tags { get; set; } = new HashSet<string>();

    public IPAddress IpAddress { get; set; } = IPAddress.Loopback;

    public int Port { get; set; }

    public TimeSpan Interval { get; set; } = TimeSpan.FromMilliseconds(3000.0);

#pragma warning disable CA1707
    public void set_IpAddress(string value)
#pragma warning restore CA1707
    {
        IpAddress = IPAddress.Parse(value);
    }
}
