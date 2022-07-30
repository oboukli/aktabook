// Copyright (c) 2022 Omar Boukli-Hacene. All rights reserved.
// Distributed under an MIT-style license that can be
// found in the LICENSE file.

// SPDX-License-Identifier: MIT

using Aktabook.Application;
using Aktabook.Application.Services;
using Aktabook.Bus.Common;
using Aktabook.Connectors.OpenLibrary;
using Aktabook.Connectors.OpenLibrary.DependencyInjection;
using Aktabook.Data;
using Aktabook.Data.Constants;
using Aktabook.Infrastructure.Configuration;
using Aktabook.Services.BookInfoRequestService;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NServiceBus;
using Serilog;
using Serilog.Exceptions;
using Serilog.Exceptions.Core;
using Serilog.Exceptions.EntityFrameworkCore.Destructurers;
using Serilog.Formatting.Compact;

const string bootstrapLogFileName = "Logs/aktabook-bus-bootstrap.log";

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .Enrich.WithExceptionDetails()
    .WriteTo.Console()
    .WriteTo.File(
        new CompactJsonFormatter(),
        bootstrapLogFileName,
        rollingInterval: RollingInterval.Infinite,
        rollOnFileSizeLimit: false,
        fileSizeLimitBytes: 32 * 1024 * 1024
    )
    .CreateBootstrapLogger();

Log.Information("Starting host");

try
{
    IHostBuilder? builder = Host.CreateDefaultBuilder(args);

    builder.UseConsoleLifetime();

    builder.ConfigureAppConfiguration(configBuilder =>
    {
        string? environmentName =
            Environment.GetEnvironmentVariable(Constants
                .AktabookEnvironmentVarName);

        configBuilder.AddJsonFile("appsettings.json", true);
        if (!string.IsNullOrEmpty(environmentName))
        {
            configBuilder.AddJsonFile($"appsettings.{environmentName}.json",
                true);
        }

        configBuilder.AddEnvironmentVariables();
    });

    builder.UseSerilog((context, services, loggerConfiguration) =>
        loggerConfiguration
            .ReadFrom.Configuration(context.Configuration)
            .Enrich.WithExceptionDetails(new DestructuringOptionsBuilder()
                .WithDefaultDestructurers()
                .WithDestructurers(
                    new[] { new DbUpdateExceptionDestructurer() })
            )
            .Enrich.WithNsbExceptionDetails()
    );

    builder.ConfigureServices((context, services) =>
    {
        services.AddDbContext<RequesterServiceDbContext>(x =>
            x.UseSqlServer(context.Configuration
                .GetSqlConnectionStringBuilderFrom(DbContextConstants
                    .RequesterServiceDbContextSqlServerSection)
                .ConnectionString));

        OpenLibraryClientOptions openLibraryClientOptions = context
            .Configuration
            .GetRequiredSection(nameof(OpenLibraryClientOptions))
            .Get<OpenLibraryClientOptions>();
        services.AddOpenLibraryClient(openLibraryClientOptions);
        services.AddScoped<IBookInfoRequestService, BookInfoRequestService>();
    });

    builder.UseNServiceBus(context =>
    {
        EndpointConfiguration endpointConfiguration =
            DefaultEndpointConfiguration.CreateDefault(Constants.Bus
                .EndpointName
                .BookInfoRequestEndpoint);

        TransportExtensions<RabbitMQTransport> transport =
            endpointConfiguration.UseTransport<RabbitMQTransport>();
        transport.ConnectionString(context.Configuration
            .GetRabbitMqBusConnectionString(Constants.Bus.Configuration
                .RequesterServiceBusSection));
        transport.UseConventionalRoutingTopology(QueueType.Quorum);

        endpointConfiguration.SendFailedMessagesTo(
            Constants.Bus.QueueName.ErrorQueue);

        endpointConfiguration.AuditProcessedMessagesTo(Constants.Bus.QueueName
            .AuditQueue);

        endpointConfiguration.DefineCriticalErrorAction(
            async criticalErrorContext =>
            {
                Log.Fatal(criticalErrorContext.Exception,
                    "Critical error: {Error}",
                    criticalErrorContext.Error);

                await criticalErrorContext.Stop()
                    .ConfigureAwait(false);

                Log.CloseAndFlush();

                string output =
                    $"NServiceBus critical error:\n{criticalErrorContext.Error}\nShutting down.";
                Environment.FailFast(output, criticalErrorContext.Exception);
            });

        return endpointConfiguration;
    });

    IHost app = builder.Build();

    app.Run();

    return 0;
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
    return 1;
}
finally
{
    Log.CloseAndFlush();
}