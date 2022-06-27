// Copyright (c) 2022 Omar Boukli-Hacene. All rights reserved.
// Distributed under an MIT-style license that can be
// found in the LICENSE file.

// SPDX-License-Identifier: MIT

using System;
using Aktabook.Data.Configuration;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Aktabook.Data.IntegrationTest.Fixtures;

public sealed class RequesterServiceDbContextSqlServerFixture : IDisposable
{
    private static readonly object Lock = new();

    private static bool _dbInitialized;

    public RequesterServiceDbContextSqlServerFixture()
    {
        if (_dbInitialized)
        {
            return;
        }

        lock (Lock)
        {
            if (_dbInitialized)
            {
                return;
            }

            SqlConnectionStringBuilder? builder = new ConfigurationFixture()
                .Configuration
                .GetSection(Constants.RequesterServiceDbContextSqlServerSection)
                .Get<SqlConnectionStringBuilder?>(options =>
                    options.ErrorOnUnknownConfiguration = true);

            if (builder is null)
            {
                throw new InvalidOperationException(
                    @$"Could not retrieve configuration section ""{Constants.RequesterServiceDbContextSqlServerSection}""");
            }

            RequesterServiceDbContext dbContext =
                CreateDbContext(builder.ConnectionString);
            dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureCreated();

            DbContext = dbContext;

            _dbInitialized = true;
        }
    }

    public RequesterServiceDbContext DbContext { get; } = null!;

    public void Dispose()
    {
        DbContext.Dispose();
    }

    private static RequesterServiceDbContext CreateDbContext(
        string connectionString)
    {
        return new RequesterServiceDbContext(
            new DbContextOptionsBuilder<RequesterServiceDbContext>()
                .UseSqlServer(connectionString)
                .Options);
    }
}