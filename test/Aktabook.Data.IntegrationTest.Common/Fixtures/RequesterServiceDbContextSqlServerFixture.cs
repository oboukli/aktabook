// Copyright (c) 2022 Omar Boukli-Hacene. All rights reserved.
// Distributed under an MIT-style license that can be
// found in the LICENSE file.

// SPDX-License-Identifier: MIT

using Aktabook.Data.Constants;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Aktabook.Data.IntegrationTest.Common.Fixtures;

public sealed class RequesterServiceDbContextSqlServerFixture : IDisposable
{
    private static readonly object Lock = new();

    private static bool _dbInitialized;

    static RequesterServiceDbContextSqlServerFixture()
    {
        _dbInitialized = false;
    }

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
                .GetRequiredSection(DbContextConstants
                    .RequesterServiceDbContextSqlServerSection)
                .Get<SqlConnectionStringBuilder?>(options =>
                    options.ErrorOnUnknownConfiguration = true);

            if (builder is null)
            {
                throw new InvalidOperationException(
                    @$"Section ""{DbContextConstants.RequesterServiceDbContextSqlServerSection}"" is not found in configuration.");
            }

            RequesterServiceDbContext dbContext =
                CreateDbContext(builder.ConnectionString);
            dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureCreated();

            DbContext = dbContext;
#pragma warning disable S3010
            _dbInitialized = true;
#pragma warning restore S3010
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
                .ConfigureWarnings(b => b.Throw())
                .UseSqlServer(connectionString)
                .Options);
    }
}