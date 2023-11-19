// Copyright (c) Omar Boukli-Hacene. All rights reserved.
// Distributed under an MIT-style license that can be
// found in the LICENSE file.

// SPDX-License-Identifier: MIT

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Aktabook.Data;
using Aktabook.Data.Testing.Fixtures;
using Aktabook.Domain.Models;
using FluentAssertions;
using FluentAssertions.Extensions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Aktabook.Services.BookInfoRequestService.IntegrationTest;

[Trait("Category", "Ephemeral")]
public class BookInfoRequesterTest : IClassFixture<RequesterServiceDbContextSqlServerDestructiveFixture>
{
    private readonly RequesterServiceDbContextSqlServerDestructiveFixture _dbDestructiveFixture;

    public BookInfoRequesterTest(
        RequesterServiceDbContextSqlServerDestructiveFixture dbDestructiveFixture)
    {
        _dbDestructiveFixture = dbDestructiveFixture;
    }

    [Fact]
    public async Task GivenPlaceRequest_WhenIsbn_ThenReturnBookInfoRequestId()
    {
        await using RequesterServiceDbContext dbContext = _dbDestructiveFixture.CreateDbContext();
        BookInfoRequester bookInfoRequester = new(dbContext);

        Guid bookInfoRequestId = await bookInfoRequester.PlaceRequest("dummy isbn", CancellationToken.None);

        bookInfoRequestId.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GivenPlaceRequest_WhenIsbn_ThenCreateOneBookInfoRequest()
    {
        await using RequesterServiceDbContext dbContext = _dbDestructiveFixture.CreateDbContext();
        BookInfoRequester bookInfoRequester = new(dbContext);

        Guid bookInfoRequestId =
            await bookInfoRequester.PlaceRequest("Dummy ISBN", CancellationToken.None);

        List<BookInfoRequest> bookInfoRequests = await dbContext
            .BookInfoRequests
            .AsNoTracking()
            .Where(x => x.BookInfoRequestId == bookInfoRequestId)
            .ToListAsync();

        bookInfoRequests.Should().ContainSingle()
            .Which.Isbn.Should().Be("Dummy ISBN");
    }

    [Fact]
    public async Task
        GivenPlaceRequest_WhenIsbn_ThenCreateOneBookInfoRequestAndOneBookInfoRequestLogEntryWithStatusSetToRequested()
    {
        await using RequesterServiceDbContext dbContext = _dbDestructiveFixture.CreateDbContext();
        BookInfoRequester bookInfoRequester = new(dbContext);

        Guid bookInfoRequestId =
            await bookInfoRequester.PlaceRequest("Dummy ISBN", CancellationToken.None);

        List<BookInfoRequest> bookInfoRequests = await dbContext
            .BookInfoRequests
            .AsNoTracking()
            .Where(x => x.BookInfoRequestId == bookInfoRequestId)
            .Include(x => x.BookInfoRequestLogEntries)
            .ToListAsync();

        BookInfoRequest expected = new() { Isbn = "Dummy ISBN" };
        expected.BookInfoRequestLogEntries.Add(new BookInfoRequestLogEntry
        {
            Status = BookInfoRequestStatus.Requested
        });
        bookInfoRequests.Should().ContainSingle()
            .Which.Should().BeEquivalentTo(expected,
                config =>
                    config
                        .Using<Guid>(ctx => ctx.Subject.Should().NotBeEmpty())
                        .WhenTypeIs<Guid>()
                        .Using<DateTime>(ctx => ctx.Subject.Should().BeWithin(1.Seconds()))
                        .WhenTypeIs<DateTime>()
                        .Excluding(x => x.BookInfoRequestLogEntries[0].BookInfoRequest)
            );
    }

    [Fact]
    public async Task GivenPlaceRequest_WhenIsbn_ThenRequestStatusIsRequested()
    {
        await using RequesterServiceDbContext dbContext = _dbDestructiveFixture.CreateDbContext();
        BookInfoRequester bookInfoRequester = new(dbContext);

        Guid bookInfoRequestId = await bookInfoRequester.PlaceRequest("Dummy ISBN", CancellationToken.None);

        List<BookInfoRequestLogEntry> bookInfoRequestLogEntries =
            await dbContext
                .BookInfoRequestLogEntries
                .AsNoTracking()
                .Where(x => x.BookInfoRequestId == bookInfoRequestId)
                .ToListAsync();

        bookInfoRequestLogEntries.Should().ContainSingle()
            .Which.Status.Should().Be(BookInfoRequestStatus.Requested);
    }

    [Fact]
    public async Task
        GivenChangeRequestStatus_WhenNewStatus_ThenNewBookInfoRequestLogEntryWithNewStatusAndReturnTrue()
    {
        await using RequesterServiceDbContext dbContext = _dbDestructiveFixture.CreateDbContext();
        BookInfoRequester bookInfoRequester = new(dbContext);

        Guid bookInfoRequestId =
            await bookInfoRequester.PlaceRequest("Dummy ISBN", CancellationToken.None);

        await bookInfoRequester
            .ChangeRequestStatus(bookInfoRequestId, "Dummy BookInfoRequestStatus", CancellationToken.None);

        List<BookInfoRequestLogEntry> bookInfoRequestLogEntries =
            await dbContext
                .BookInfoRequestLogEntries
                .AsNoTracking()
                .Where(x => x.BookInfoRequestId == bookInfoRequestId)
                .ToListAsync();

        bookInfoRequestLogEntries.Should().HaveCount(2)
            .And.ContainSingle(x => x.Status == BookInfoRequestStatus.Requested)
            .And.ContainSingle(x =>
                x.Status.Equals("Dummy BookInfoRequestStatus", StringComparison.Ordinal));
    }

    [Fact]
    public async Task
        GivenChangeRequestStatus_WhenSameStatus_ThenReturnFalseWithoutAddingBookInfoRequestLogEntry()
    {
        await using RequesterServiceDbContext dbContext = _dbDestructiveFixture.CreateDbContext();
        BookInfoRequester bookInfoRequester = new(dbContext);

        Guid bookInfoRequestId =
            await bookInfoRequester.PlaceRequest("Dummy ISBN", CancellationToken.None);

        await bookInfoRequester
            .ChangeRequestStatus(bookInfoRequestId, BookInfoRequestStatus.Requested, CancellationToken.None);

        List<BookInfoRequestLogEntry> bookInfoRequestLogEntries =
            await dbContext
                .BookInfoRequestLogEntries
                .AsNoTracking()
                .Where(x => x.BookInfoRequestId == bookInfoRequestId)
                .ToListAsync();

        bookInfoRequestLogEntries.Should()
            .ContainSingle()
            .Which.Status.Should().Be(BookInfoRequestStatus.Requested);
    }

    [Fact]
    public async Task GivenChangeRequestStatus_WhenMultipleCalls_ThenCorrectEntries()
    {
        await using RequesterServiceDbContext dbContext = _dbDestructiveFixture.CreateDbContext();
        BookInfoRequester bookInfoRequester = new(dbContext);

        Guid bookInfoRequestId =
            await bookInfoRequester.PlaceRequest("Dummy ISBN", CancellationToken.None);

        await bookInfoRequester
            .ChangeRequestStatus(bookInfoRequestId, "Dummy status 001", CancellationToken.None);

        await bookInfoRequester
            .ChangeRequestStatus(bookInfoRequestId, "Dummy status 002", CancellationToken.None);

        await bookInfoRequester
            .ChangeRequestStatus(bookInfoRequestId, "Dummy status 003", CancellationToken.None);

        await bookInfoRequester
            .ChangeRequestStatus(bookInfoRequestId, "Dummy status 004", CancellationToken.None);

        List<BookInfoRequestLogEntry> bookInfoRequestLogEntries =
            await dbContext
                .BookInfoRequestLogEntries
                .AsNoTracking()
                .Where(x => x.BookInfoRequestId == bookInfoRequestId)
                .ToListAsync();

        bookInfoRequestLogEntries.Should().HaveCount(5)
            .And.ContainSingle(x => x.Status == BookInfoRequestStatus.Requested)
            .And.ContainSingle(x => x.Status.Equals("Dummy status 004", StringComparison.Ordinal));
    }
}
