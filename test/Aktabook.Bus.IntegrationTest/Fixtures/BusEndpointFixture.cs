// Copyright (c) 2022 Omar Boukli-Hacene. All rights reserved.
// Distributed under an MIT-style license that can be
// found in the LICENSE file.

// SPDX-License-Identifier: MIT

using System.Threading.Tasks;
using Aktabook.Application;
using Aktabook.Application.Messages.Commands;
using Aktabook.Bus.Common;
using Aktabook.Data.Testing.Fixtures;
using Aktabook.Infrastructure.Configuration;
using NServiceBus;

namespace Aktabook.Bus.IntegrationTest.Fixtures;

public class BusEndpointFixture
{
    private readonly EndpointConfiguration _endpointConfiguration;

    private IEndpointInstance? _endpointInstance;

    public BusEndpointFixture()
    {
        string rabbitMqBusConnectionString = new ConfigurationFixture()
            .Configuration
            .GetRabbitMqBusConnectionString(Constants.Bus.Configuration
                .RequesterServiceBusSection);

        EndpointConfiguration endpointConfiguration =
            DefaultEndpointConfiguration.CreateDefault("SenderTestEndpoint");

        TransportExtensions<RabbitMQTransport> transport =
            endpointConfiguration.UseTransport<RabbitMQTransport>();
        transport.ConnectionString(rabbitMqBusConnectionString);
        transport.UseConventionalRoutingTopology(QueueType.Quorum);
        transport.Routing().RouteToEndpoint(typeof(ProcessBookInfoRequest),
            Constants.Bus.EndpointName.BookInfoRequestEndpoint);

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
