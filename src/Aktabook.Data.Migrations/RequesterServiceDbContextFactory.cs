// Copyright (c) 2022 Omar Boukli-Hacene. All rights reserved.
// Distributed under an MIT-style license that can be
// found in the LICENSE file.

// SPDX-License-Identifier: MIT

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Aktabook.Data.Migrations;

public class
    RequesterServiceDbContextFactory : IDesignTimeDbContextFactory<
        RequesterServiceDbContext>
{
    public RequesterServiceDbContext CreateDbContext(string[] args)
    {
        DbContextOptionsBuilder<RequesterServiceDbContext> optionsBuilder =
            new DbContextOptionsBuilder<RequesterServiceDbContext>()
                .UseSqlServer(x =>
                    x.MigrationsAssembly("Aktabook.Data.Migrations"));

        return new RequesterServiceDbContext(optionsBuilder.Options);
    }
}
