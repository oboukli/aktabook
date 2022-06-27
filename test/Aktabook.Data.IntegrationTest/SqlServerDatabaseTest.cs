// Copyright (c) 2022 Omar Boukli-Hacene. All rights reserved.
// Distributed under an MIT-style license that can be
// found in the LICENSE file.

// SPDX-License-Identifier: MIT

using System.Linq;
using Aktabook.Data.IntegrationTest.Fixtures;
using Xunit;

namespace Aktabook.Data.IntegrationTest;

public class
    SqlServerDatabaseTest : IClassFixture<
        RequesterServiceDbContextSqlServerFixture>
{
    private readonly RequesterServiceDbContextSqlServerFixture
        _requesterServiceDbContextSqlServerFixture;

    public SqlServerDatabaseTest(
        RequesterServiceDbContextSqlServerFixture
            requesterServiceDbContextSqlServerFixture)
    {
        _requesterServiceDbContextSqlServerFixture =
            requesterServiceDbContextSqlServerFixture;
    }

    [Fact]
    public void
        GivenDatabaseContextDatabase_WhenCreateContext_ThenCreateDatabase()
    {
        _requesterServiceDbContextSqlServerFixture.DbContext.Authors
            .SingleOrDefault();
    }
}