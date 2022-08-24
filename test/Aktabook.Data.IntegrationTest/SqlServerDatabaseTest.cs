// Copyright (c) 2022 Omar Boukli-Hacene. All rights reserved.
// Distributed under an MIT-style license that can be
// found in the LICENSE file.

// SPDX-License-Identifier: MIT

using System.Linq;
using Aktabook.Data.Testing.Fixtures;
using Aktabook.Domain.Models;
using FluentAssertions;
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
        Author? singleOrDefault = _requesterServiceDbContextSqlServerFixture
            .DbContext.Authors
            .SingleOrDefault();

        singleOrDefault.Should().BeNull("Dataset is empty");
    }
}