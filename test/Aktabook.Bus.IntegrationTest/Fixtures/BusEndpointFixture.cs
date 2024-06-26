// Copyright (c) Omar Boukli-Hacene. All rights reserved.
// Distributed under an MIT-style license that can be
// found in the LICENSE file.

// SPDX-License-Identifier: MIT

using System.Threading.Tasks;
using Aktabook.Application;
using Aktabook.Application.Messages.Commands;
using Aktabook.Data.Testing.Fixtures;
using Aktabook.Infrastructure.BusInfra;
using Aktabook.Infrastructure.Configuration;
using NServiceBus;

namespace Aktabook.Bus.IntegrationTest.Fixtures;

public class BusEndpointFixture
{
    private readonly EndpointConfiguration _endpointConfiguration;

    private IEndpointInstance? _endpointInstance;

    public BusEndpointFixture()
    {
        string rabbitMqBusConnectionString = new ConfigurationFactory()
            .Configuration
            .GetRabbitMqBusConnectionString(BusConfiguration.RequesterServiceBusSection);

        EndpointConfiguration endpointConfiguration =
            DefaultEndpointConfiguration.CreateDefault("SenderTestEndpoint");

        TransportExtensions<RabbitMQTransport> transport =
            endpointConfiguration.UseTransport<RabbitMQTransport>();
        transport.ConnectionString(rabbitMqBusConnectionString);
        transport.UseConventionalRoutingTopology(QueueType.Quorum);
        transport.Routing()
            .RouteToEndpoint(typeof(ProcessBookInfoRequest), BusEndpointName.BookInfoRequestEndpoint);

        endpointConfiguration.SendOnly();
        _endpointConfiguration = endpointConfiguration;
    }

    public async ValueTask<IEndpointInstance> GetEndpointInstance()
    {
        if (_endpointInstance is { })
        {
            return _endpointInstance;
        }

        _endpointInstance = await Endpoint.Start(_endpointConfiguration).ConfigureAwait(false);

        return _endpointInstance;
    }
}
