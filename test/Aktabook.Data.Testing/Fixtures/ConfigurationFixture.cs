// Copyright (c) 2022 Omar Boukli-Hacene. All rights reserved.
// Distributed under an MIT-style license that can be
// found in the LICENSE file.

// SPDX-License-Identifier: MIT

using Microsoft.Extensions.Configuration;

namespace Aktabook.Data.Testing.Fixtures;

public class ConfigurationFixture
{
    private IConfiguration? _configuration;

    public IConfiguration Configuration
    {
        get
        {
            if (_configuration is { })
            {
                return _configuration;
            }

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
            _configuration = configBuilder.Build();

            return _configuration;
        }
    }
}
