// Copyright (c) Omar Boukli-Hacene. All rights reserved.
// Distributed under an MIT-style license that can be
// found in the LICENSE file.

// SPDX-License-Identifier: MIT

using Aktabook.Data.Constants;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Aktabook.Data.Testing.Fixtures;

public sealed class RequesterServiceDbContextSqlServerDestructiveFixture
{
    private static readonly object Lock = new();
    private static bool _dbInitialized;

    private string? _connectionString;

    public RequesterServiceDbContextSqlServerDestructiveFixture()
    {
        lock (Lock)
        {
            if (!_dbInitialized)
            {
                using (RequesterServiceDbContext dbContext = CreateDbContext())
                {
                    dbContext.Database.EnsureDeleted();
                    dbContext.Database.EnsureCreated();
                }

#pragma warning disable S3010
                _dbInitialized = true;
#pragma warning restore S3010
            }
        }
    }

    public RequesterServiceDbContext CreateDbContext()
    {
        if (_connectionString is null)
        {
            _connectionString = new ConfigurationFactory()
                .Configuration
                .GetRequiredSection(DbContextConstants.RequesterServiceDbContextSqlServerSection)
                .Get<SqlConnectionStringBuilder>(options => options.ErrorOnUnknownConfiguration = true)!
                .ConnectionString;
        }

        return new RequesterServiceDbContext(
            new DbContextOptionsBuilder<RequesterServiceDbContext>()
                .ConfigureWarnings(b => b.Throw())
                .UseSqlServer(_connectionString)
                .Options);
    }
}
