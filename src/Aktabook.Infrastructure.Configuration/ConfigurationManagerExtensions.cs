// Copyright (c) Omar Boukli-Hacene. All rights reserved.
// Distributed under an MIT-style license that can be
// found in the LICENSE file.

// SPDX-License-Identifier: MIT

using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Aktabook.Infrastructure.Configuration;

public static class ConfigurationManagerExtensions
{
    public static SqlConnectionStringBuilder GetSqlConnectionStringBuilderFrom(
        this IConfiguration configuration, string section)
    {
        return configuration
            .GetRequiredSection(section)
            .Get<SqlConnectionStringBuilder>(options => options.ErrorOnUnknownConfiguration = true);
    }

    public static string GetRabbitMqBusConnectionString(this IConfiguration configuration, string section)
    {
        return configuration
            .GetRequiredSection(section)
            .Get<AmqpUriBuilder>().ConnectionUri.ToString();
    }
}
