// Copyright (c) Omar Boukli-Hacene. All rights reserved.
// Distributed under an MIT-style license that can be
// found in the LICENSE file.

// SPDX-License-Identifier: MIT

using Aktabook.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Aktabook.Data;

public class RequesterServiceDbContext : DbContext
{
    public RequesterServiceDbContext()
    {
    }

    public RequesterServiceDbContext(DbContextOptions options) : base(options)
    {
    }

    public virtual DbSet<Author> Authors => Set<Author>();

    public virtual DbSet<Book> Books => Set<Book>();

    public virtual DbSet<BookInfoRequest> BookInfoRequests => Set<BookInfoRequest>();

    public virtual DbSet<BookInfoRequestLogEntry> BookInfoRequestLogEntries => Set<BookInfoRequestLogEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

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
            .WithOne(e => e.BookInfoRequest);

        modelBuilder.Entity<BookInfoRequestLogEntry>()
            .ToTable("BookInfoRequestLogEntry")
            .HasIndex(e => e.Status);
    }
}
