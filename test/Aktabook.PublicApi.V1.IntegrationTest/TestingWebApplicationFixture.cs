// Copyright (c) Omar Boukli-Hacene. All rights reserved.
// Distributed under an MIT-style license that can be
// found in the LICENSE file.

// SPDX-License-Identifier: MIT

using System;
using System.IO;
using System.Net.Http;
using Aktabook.Data;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Aktabook.PublicApi.V1.IntegrationTest;

public class TestingWebApplicationFixture<TStartup> where TStartup : class
{
    private readonly WebApplicationFactory<TStartup> _application;
    private readonly IServiceScope _serviceScope;

    public TestingWebApplicationFixture()
    {
        _application = new WebApplicationFactory<TStartup>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureAppConfiguration((context, configBuilder) =>
                {
                    configBuilder.Sources.Clear();
                    string? environmentName =
                        Environment.GetEnvironmentVariable("AKTABOOK_INTEGRATION_TEST_ENVIRONMENT");

                    configBuilder.SetBasePath(Directory.GetCurrentDirectory());
                    configBuilder.AddJsonFile("appsettings.json", true);
                    if (!string.IsNullOrEmpty(environmentName))
                    {
                        configBuilder.AddJsonFile($"appsettings.{environmentName}.json", true);
                    }

                    configBuilder.AddEnvironmentVariables();
                });
            });

        _serviceScope = _application.Server.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
        RequesterServiceDbContext =
            _serviceScope.ServiceProvider.GetRequiredService<RequesterServiceDbContext>();
    }

    public RequesterServiceDbContext RequesterServiceDbContext { get; }

    public HttpClient CreateClient()
    {
        return _application.CreateClient();
    }
}
