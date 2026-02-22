// Copyright (c) Omar Boukli-Hacene. All rights reserved.
// Distributed under an MIT-style license that can be
// found in the LICENSE file.

// SPDX-License-Identifier: MIT

using Aktabook.Application;
using Aktabook.Data.Constants;
using Aktabook.Diagnostics.HealthChecks;
using Aktabook.Diagnostics.Process;
using Aktabook.Infrastructure.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using RabbitMQ.Client;

namespace Aktabook.Bus;

internal static class ServiceCollectionExtensions
{
    private const string Liveness = "liveness";
    private const string Readiness = "readiness";

    internal static IHealthChecksBuilder AddBusHealthChecks(this IServiceCollection services,
        IConfiguration configurationRoot)
    {
        return services.AddHealthChecks()
            .AddSqlServerHealthChecks(configurationRoot)
            .AddCheck("Self", () => HealthCheckResult.Healthy(), [Liveness, Readiness])
            .AddRabbitMqHealthChecks(configurationRoot);
    }

    internal static void AddHealthCheckTcpListenerServices(this IServiceCollection services,
        IConfiguration configurationRoot)
    {
        services.AddOptions<LivenessListenerOptions>()
            .Bind(configurationRoot
                .GetSection(nameof(LivenessListenerOptions)));

        services.AddOptions<ReadinessListenerOptions>()
            .Bind(configurationRoot
                .GetSection(nameof(ReadinessListenerOptions)));

        services.AddHealthCheckTcpEndpoint();
    }

    internal static void AddProcessIdFileHostedService(
        this IServiceCollection services,
        IConfiguration configurationRoot)
    {
        services.AddOptions<ProcessIdFileHostedServiceOptions>()
            .Bind(configurationRoot
                .GetRequiredSection(nameof(ProcessIdFileHostedServiceOptions)));
        services.AddHostedService<ProcessIdFileHostedService>();
    }

    private static IHealthChecksBuilder AddRabbitMqHealthChecks(
        this IHealthChecksBuilder healthChecksBuilder,
        IConfiguration configurationRoot)
    {
        return healthChecksBuilder.AddRabbitMQ(async _ =>
            {
                var factory = new ConnectionFactory
                {
                    Uri = new Uri(configurationRoot
                        .GetRabbitMqBusConnectionString(BusConfiguration.RequesterServiceBusSection)),
                    AutomaticRecoveryEnabled = true
                };
                return await factory.CreateConnectionAsync().ConfigureAwait(false);
            },
        name: "RabbitMQ",
        tags: [Readiness]);
    }

    private static IHealthChecksBuilder AddSqlServerHealthChecks(
        this IHealthChecksBuilder healthChecksBuilder,
        IConfiguration configurationRoot)
    {
        const string healthQuery = "SET NOCOUNT ON; SELECT TOP(0) 1 FROM [BookInfoRequest]";

        return healthChecksBuilder.AddSqlServer(
            configurationRoot
                .GetSqlConnectionStringBuilderFrom(DbContextConstants
                    .RequesterServiceDbContextSqlServerSection)
                .ConnectionString, healthQuery, name: "SqlServer",
            tags: [Readiness]);
    }
}
