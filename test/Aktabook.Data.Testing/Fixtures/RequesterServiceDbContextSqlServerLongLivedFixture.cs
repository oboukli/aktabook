// Copyright (c) 2022 Omar Boukli-Hacene. All rights reserved.
// Distributed under an MIT-style license that can be
// found in the LICENSE file.

// SPDX-License-Identifier: MIT

using Aktabook.Data.Constants;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Aktabook.Data.Testing.Fixtures;

public sealed class
    RequesterServiceDbContextSqlServerLongLivedFixture : IDisposable
{
    public RequesterServiceDbContextSqlServerLongLivedFixture()
    {
        SqlConnectionStringBuilder builder = new ConfigurationFixture()
            .Configuration
            .GetRequiredSection(DbContextConstants
                .RequesterServiceDbContextSqlServerSection)
            .Get<SqlConnectionStringBuilder>(options =>
                options.ErrorOnUnknownConfiguration = true);

        DbContext = CreateDbContext(builder.ConnectionString);
    }

    public RequesterServiceDbContext DbContext { get; }

    public void Dispose()
    {
        DbContext.Dispose();
    }

    private static RequesterServiceDbContext CreateDbContext(
        string connectionString)
    {
        return new RequesterServiceDbContext(
            new DbContextOptionsBuilder<RequesterServiceDbContext>()
                .ConfigureWarnings(b => b.Throw())
                .UseSqlServer(connectionString)
                .Options);
    }
}