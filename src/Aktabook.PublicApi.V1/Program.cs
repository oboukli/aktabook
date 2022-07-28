// Copyright (c) 2022 Omar Boukli-Hacene. All rights reserved.
// Distributed under an MIT-style license that can be
// found in the LICENSE file.

// SPDX-License-Identifier: MIT

using Aktabook.Application;
using Aktabook.Data.Constants;
using Aktabook.Infrastructure.Configuration;
using Aktabook.PublicApi.V1.DependencyInjection;
using Aktabook.PublicApi.V1.Validators;
using FluentValidation.AspNetCore;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Exceptions;
using Serilog.Exceptions.Core;
using Serilog.Exceptions.EntityFrameworkCore.Destructurers;
using Serilog.Formatting.Compact;

const string bootstrapLogFileName = "aktabook-api-bootstrap.log";

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
    WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((context, services, loggerConfiguration) =>
        loggerConfiguration.ReadFrom.Configuration(context.Configuration)
            .Enrich.WithExceptionDetails(new DestructuringOptionsBuilder()
                .WithDefaultDestructurers()
                .WithDestructurers(
                    new[] { new DbUpdateExceptionDestructurer() })
            )
    );

    builder.Services.AddControllers()
        .AddFluentValidation(x =>
        {
            x.DisableDataAnnotationsValidation = true;
            x.RegisterValidatorsFromAssemblyContaining<
                CreateBookInfoRequestRequestValidator>();
        });

    builder.Services.AddApplicationServices(options =>
        options.UseSqlServer(builder.Configuration
            .GetSqlConnectionStringBuilderFrom(DbContextConstants
                .RequesterServiceDbContextSqlServerSection).ConnectionString));

    builder.Services.AddHealthChecks();
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Version = "v1",
            Title = "Aktabook",
            Description = "A book data aggregator API",
            License = new OpenApiLicense
            {
                Name = "MIT",
                Url = new Uri(Constants.AppLicenseUrl)
            }
        });
    });
    builder.Services.AddFluentValidationRulesToSwagger();

    WebApplication app = builder.Build();

    app.UseRouting();
    app.UseEndpoints(configure =>
    {
        configure.MapControllers();
    });
    app.UseHealthChecks("/healthz");

    app.UseSwagger();
    app.UseSwaggerUI();

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