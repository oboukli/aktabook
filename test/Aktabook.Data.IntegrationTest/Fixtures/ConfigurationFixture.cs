// Copyright (c) 2022 Omar Boukli-Hacene. All rights reserved.
// Distributed under an MIT-style license that can be
// found in the LICENSE file.

// SPDX-License-Identifier: MIT

using System;
using Microsoft.Extensions.Configuration;

namespace Aktabook.Data.IntegrationTest.Fixtures;

public class ConfigurationFixture
{
    public ConfigurationFixture()
    {
        string? environmentName =
            Environment.GetEnvironmentVariable(
                "AKTABOOK_INTEGRATION_TEST_ENVIRONMENT");

        ConfigurationBuilder configBuilder = new();
        configBuilder.AddJsonFile("appsettings.json", true);
        if (!string.IsNullOrEmpty(environmentName))
        {
            configBuilder.AddJsonFile($"appsettings.{environmentName}.json",
                true);
        }

        configBuilder.AddEnvironmentVariables();
        Configuration = configBuilder.Build();
    }

    public IConfiguration Configuration { get; }
}