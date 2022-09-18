// Copyright (c) 2022 Omar Boukli-Hacene. All rights reserved.
// Distributed under an MIT-style license that can be
// found in the LICENSE file.

// SPDX-License-Identifier: MIT

using Aktabook.Data.Constants;
using Aktabook.Diagnostics.HealthChecks;
using Aktabook.Diagnostics.Process;
using Aktabook.Infrastructure.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using RabbitMQ.Client;

namespace Aktabook.Bus;

public static class ServiceCollectionExtensions
{
    private const string Liveness = "liveness";
    private const string Readiness = "readiness";

    public static void AddBusHealthChecks(this IServiceCollection services, IConfiguration configurationRoot)
    {
        services.AddHealthChecks()
            .AddRabbitMqHealthChecks(configurationRoot)
            .AddSqlServerHealthChecks(configurationRoot)
            .AddCheck("Self", () => HealthCheckResult.Healthy(), new[] { Liveness, Readiness });
    }

    public static void AddHealthCheckTcpListenerServices(this IServiceCollection services,
        IConfiguration configurationRoot)
    {
        services.AddOptions<LivenessListenerOptions>()
            .Bind(configurationRoot
                .GetSection(nameof(LivenessListenerOptions)));

        services.AddOptions<ReadinessListenerOptions>()
            .Bind(configurationRoot
                .GetSection(nameof(ReadinessListenerOptions)));

        services.AddHealthCheckTcpEndpoint(configurationRoot);
    }

    public static void AddProcessIdFileHostedService(
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
        return healthChecksBuilder.AddRabbitMQ(configurationRoot
                .GetRabbitMqBusConnectionString(Application.Constants.Bus.Configuration
                    .RequesterServiceBusSection),
            new SslOption(), "RabbitMQ", tags: new[] { Readiness });
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
                .ConnectionString, healthQuery, "SqlServer",
            tags: new[] { Readiness });
    }
}