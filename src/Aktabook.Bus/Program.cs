// Copyright (c) Omar Boukli-Hacene. All rights reserved.
// Distributed under an MIT-style license that can be
// found in the LICENSE file.

// SPDX-License-Identifier: MIT

using System.Diagnostics;
using System.Globalization;
using Aktabook.Application;
using Aktabook.Application.Services;
using Aktabook.Bus;
using Aktabook.Bus.Common;
using Aktabook.Connectors.OpenLibrary;
using Aktabook.Connectors.OpenLibrary.DependencyInjection;
using Aktabook.Data;
using Aktabook.Data.Constants;
using Aktabook.Diagnostics.OpenTelemetry;
using Aktabook.Infrastructure.Configuration;
using Aktabook.Services.BookInfoRequestService;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Exceptions;
using Serilog.Exceptions.Core;
using Serilog.Exceptions.EntityFrameworkCore.Destructurers;
using Serilog.Formatting.Compact;

#pragma warning disable CA1812

const string bootstrapLogFileName = "Logs/aktabook-bus-bootstrap.log";

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .Enrich.WithExceptionDetails()
    .WriteTo.Console(formatProvider: CultureInfo.InvariantCulture)
    .WriteTo.File(
        new CompactJsonFormatter(),
        bootstrapLogFileName,
        fileSizeLimitBytes: 32 * 1024 * 1024,
        rollingInterval: RollingInterval.Infinite,
        rollOnFileSizeLimit: false)
    .CreateBootstrapLogger();

Log.Information("Starting host");

try
{
    IHostBuilder builder = Host.CreateDefaultBuilder(args);

    builder.UseConsoleLifetime();

    builder.ConfigureAppConfiguration(configBuilder =>
    {
        string? environmentName = Environment.GetEnvironmentVariable(Constants.AktabookEnvironmentVarName);

        configBuilder.AddJsonFile("appsettings.json", true);
        configBuilder.AddJsonFile($"appsettings.{environmentName}.json", true);

        configBuilder.AddEnvironmentVariables();
    });

    builder.UseSerilog((context, _, loggerConfiguration) =>
        loggerConfiguration
            .ReadFrom.Configuration(context.Configuration)
            .Enrich.WithExceptionDetails(new DestructuringOptionsBuilder()
                .WithDefaultDestructurers()
                .WithDestructurers(new[] { new DbUpdateExceptionDestructurer() })
            )
            .Enrich.WithNsbExceptionDetails()
    );

    builder.ConfigureServices((context, services) =>
    {
        TelemetryOptions telemetryOptions = context
            .Configuration
            .GetRequiredSection(nameof(TelemetryOptions))
            .Get<TelemetryOptions>()!;

        services.AddSingleton(new ActivitySource(telemetryOptions.ServiceName));
        services.AddOpenTelemetry()
            .WithTracing(tracerProviderBuilder => tracerProviderBuilder
                .AddSource(telemetryOptions.ServiceName)
                .ConfigureResource(resourceBuilder => resourceBuilder
                    .AddService(telemetryOptions.ServiceName, serviceVersion: telemetryOptions.ServiceVersion)
                )
                .AddOtlpExporter()
                .AddConsoleExporter()
                .AddHttpClientInstrumentation()
            );

        IConfiguration configuration = context.Configuration;
        services.AddDbContext<RequesterServiceDbContext>(x =>
            x.UseSqlServer(configuration.GetSqlConnectionStringBuilderFrom(
                DbContextConstants.RequesterServiceDbContextSqlServerSection).ConnectionString));

        OpenLibraryClientOptions openLibraryClientOptions = context
            .Configuration
            .GetRequiredSection(nameof(OpenLibraryClientOptions))
            .Get<OpenLibraryClientOptions>()!;
        services.AddOpenLibraryClient(openLibraryClientOptions);
        services.AddScoped<IBookInfoRequester, BookInfoRequester>();

        services.AddBusHealthChecks(configuration);
        services.AddHealthCheckTcpListenerServices(configuration);
        services.AddProcessIdFileHostedService(configuration);
    });

    builder.UseNServiceBus(context =>
    {
        EndpointConfiguration endpointConfiguration =
            DefaultEndpointConfiguration.CreateDefault(BusEndpointName.BookInfoRequestEndpoint);

        TransportExtensions<RabbitMQTransport> transport =
            endpointConfiguration.UseTransport<RabbitMQTransport>();
        transport.ConnectionString(context.Configuration
            .GetRabbitMqBusConnectionString(BusConfiguration.RequesterServiceBusSection));
        transport.UseConventionalRoutingTopology(QueueType.Quorum);

        endpointConfiguration.SendFailedMessagesTo(BusQueueName.ErrorQueue);

        endpointConfiguration.AuditProcessedMessagesTo(BusQueueName.AuditQueue);

        endpointConfiguration.DefineCriticalErrorAction(async (criticalErrorContext, cancellationToken) =>
        {
            Log.Fatal(criticalErrorContext.Exception, "Critical error: {Error}", criticalErrorContext.Error);

            await criticalErrorContext.Stop(cancellationToken).ConfigureAwait(false);

            await Log.CloseAndFlushAsync().ConfigureAwait(false);

            string output = $"NServiceBus critical error:\n{criticalErrorContext.Error}\nShutting down.";
            Environment.FailFast(output, criticalErrorContext.Exception);
        });

        return endpointConfiguration;
    });

    IHost app = builder.Build();

    app.Run();

    Log.Information("Host stopped");

    return 0;
}
#pragma warning disable CA1031
catch (Exception ex)
#pragma warning restore CA1031
{
    Log.Fatal(ex, "Host terminated unexpectedly");
    return 1;
}
finally
{
    Log.CloseAndFlush();
}

#pragma warning restore CA1812
