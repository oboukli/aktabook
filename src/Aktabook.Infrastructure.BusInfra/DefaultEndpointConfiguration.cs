// Copyright (c) Omar Boukli-Hacene. All rights reserved.
// Distributed under an MIT-style license that can be
// found in the LICENSE file.

// SPDX-License-Identifier: MIT

namespace Aktabook.Infrastructure.BusInfra;

public static class DefaultEndpointConfiguration
{
    public static EndpointConfiguration CreateDefault(string endpointName)
    {
        EndpointConfiguration endpointConfiguration = new(endpointName);

        endpointConfiguration.Conventions().Add(new BusMessageConvention());

        endpointConfiguration.UseSerialization<NewtonsoftJsonSerializer>();

        return endpointConfiguration;
    }
}
