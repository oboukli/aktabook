// Copyright (c) 2022 Omar Boukli-Hacene. All rights reserved.
// Distributed under an MIT-style license that can be
// found in the LICENSE file.

// SPDX-License-Identifier: MIT

using NServiceBus;
using NServiceBus.Json;

namespace Aktabook.Bus.Common;

public static class DefaultEndpointConfiguration
{
    public static EndpointConfiguration CreateDefault(string endpointName)
    {
        EndpointConfiguration endpointConfiguration = new(endpointName);

        endpointConfiguration.Conventions().Add(new BusMessageConvention());

        endpointConfiguration.UseSerialization<SystemJsonSerializer>();

        return endpointConfiguration;
    }
}
