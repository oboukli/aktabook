// Copyright (c) 2022 Omar Boukli-Hacene. All rights reserved.
// Distributed under an MIT-style license that can be
// found in the LICENSE file.

// SPDX-License-Identifier: MIT

using Aktabook.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Aktabook.Data;

public abstract class RequesterServiceDbContext : DbContext
{
    public DbSet<Author> Authors => Set<Author>();

    public DbSet<Book> Books => Set<Book>();

    public DbSet<BookInfoRequest> BookInfoRequests => Set<BookInfoRequest>();

    public DbSet<BookInfoRequestLogEntry> BookInfoRequestLogEntries =>
        Set<BookInfoRequestLogEntry>();

    protected RequesterServiceDbContext(DbContextOptions options) :
        base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Book>()
            .ToTable("Book")
            .HasIndex(b => b.Isbn)
            .IsUnique();

        modelBuilder.Entity<Book>()
            .HasMany(b => b.Authors)
            .WithMany(a => a.Books);

        modelBuilder.Entity<Author>()
            .ToTable("Author")
            .HasIndex(a => a.Name);

        modelBuilder.Entity<BookInfoRequest>()
            .ToTable("BookInfoRequest")
            .HasIndex(r => r.Isbn);

        modelBuilder.Entity<BookInfoRequest>()
            .HasMany(r => r.BookInfoRequestLogEntries)
            .WithOne(e => e.BookInfoRequest)
            .IsRequired();

        modelBuilder.Entity<BookInfoRequestLogEntry>()
            .ToTable("BookInfoRequestLogEntry")
            .HasIndex(e => e.Status);
    }
}