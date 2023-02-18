// Copyright (c) 2022 Omar Boukli-Hacene. All rights reserved.
// Distributed under an MIT-style license that can be
// found in the LICENSE file.

// SPDX-License-Identifier: MIT

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Aktabook.Data.Testing.Fixtures;
using Aktabook.Domain.Models;
using FluentAssertions;
using FluentAssertions.Extensions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Aktabook.Services.BookInfoRequestService.IntegrationTest;

[Trait("Category", "Ephemeral")]
public class BookInfoRequesterServiceTest : IClassFixture<RequesterServiceDbContextSqlServerDestructiveFixture>
{
    private readonly RequesterServiceDbContextSqlServerDestructiveFixture _dbDestructiveFixture;

    public BookInfoRequesterServiceTest(RequesterServiceDbContextSqlServerDestructiveFixture dbDestructiveFixture)
    {
        _dbDestructiveFixture = dbDestructiveFixture;
    }

    [Fact]
    public async Task GivenPlaceRequest_WhenIsbn_ThenReturnBookInfoRequestId()
    {
        BookInfoRequestService service = new(_dbDestructiveFixture.DbContext);

        Guid bookInfoRequestId = await service.PlaceRequest("dummy isbn", CancellationToken.None)
            .ConfigureAwait(false);

        bookInfoRequestId.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GivenPlaceRequest_WhenIsbn_ThenCreateOneBookInfoRequest()
    {
        BookInfoRequestService service = new(_dbDestructiveFixture.DbContext);

        Guid bookInfoRequestId = await service.PlaceRequest("Dummy ISBN", CancellationToken.None).ConfigureAwait(false);

        List<BookInfoRequest> bookInfoRequests = await _dbDestructiveFixture
            .DbContext
            .BookInfoRequests
            .AsNoTracking()
            .Where(x => x.BookInfoRequestId == bookInfoRequestId)
            .ToListAsync()
            .ConfigureAwait(false);

        bookInfoRequests.Should().ContainSingle()
            .Which.Isbn.Should().Be("Dummy ISBN");
    }

    [Fact]
    public async Task
        GivenPlaceRequest_WhenIsbn_ThenCreateOneBookInfoRequestAndOneBookInfoRequestLogEntryWithStatusSetToRequested()
    {
        BookInfoRequestService service = new(_dbDestructiveFixture.DbContext);

        Guid bookInfoRequestId = await service.PlaceRequest("Dummy ISBN", CancellationToken.None).ConfigureAwait(false);

        List<BookInfoRequest> bookInfoRequests = await _dbDestructiveFixture
            .DbContext
            .BookInfoRequests
            .AsNoTracking()
            .Where(x => x.BookInfoRequestId == bookInfoRequestId)
            .Include(x => x.BookInfoRequestLogEntries)
            .ToListAsync()
            .ConfigureAwait(false);

        bookInfoRequests.Should().ContainSingle()
            .Which.Should().BeEquivalentTo(
                new BookInfoRequest
                {
                    Isbn = "Dummy ISBN",
                    BookInfoRequestLogEntries =
                        new List<BookInfoRequestLogEntry>
                        {
                            new() { Status = BookInfoRequestStatus.Requested }
                        }
                }, config =>
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
        BookInfoRequestService service = new(_dbDestructiveFixture.DbContext);

        Guid bookInfoRequestId = await service.PlaceRequest("Dummy ISBN", CancellationToken.None)
            .ConfigureAwait(false);

        List<BookInfoRequestLogEntry> bookInfoRequestLogEntries =
            await _dbDestructiveFixture.DbContext
                .BookInfoRequestLogEntries
                .AsNoTracking()
                .Where(x => x.BookInfoRequestId == bookInfoRequestId)
                .ToListAsync()
                .ConfigureAwait(false);

        bookInfoRequestLogEntries.Should().ContainSingle()
            .Which.Status.Should().Be(BookInfoRequestStatus.Requested);
    }

    [Fact]
    public async Task
        GivenChangeRequestStatus_WhenNewStatus_ThenNewBookInfoRequestLogEntryWithNewStatusAndReturnTrue()
    {
        BookInfoRequestService service = new(_dbDestructiveFixture.DbContext);

        Guid bookInfoRequestId = await service.PlaceRequest("Dummy ISBN", CancellationToken.None).ConfigureAwait(false);

        await service.ChangeRequestStatus(bookInfoRequestId, "Dummy BookInfoRequestStatus", CancellationToken.None).ConfigureAwait(false);

        List<BookInfoRequestLogEntry> bookInfoRequestLogEntries =
            await _dbDestructiveFixture.DbContext
                .BookInfoRequestLogEntries
                .AsNoTracking()
                .Where(x => x.BookInfoRequestId == bookInfoRequestId)
                .ToListAsync()
                .ConfigureAwait(false);

        bookInfoRequestLogEntries.Should().HaveCount(2)
            .And.ContainSingle(x => x.Status == BookInfoRequestStatus.Requested)
            .And.ContainSingle(x =>
                x.Status.Equals("Dummy BookInfoRequestStatus", StringComparison.Ordinal));
    }

    [Fact]
    public async Task
        GivenChangeRequestStatus_WhenSameStatus_ThenReturnFalseWithoutAddingBookInfoRequestLogEntry()
    {
        BookInfoRequestService service = new(_dbDestructiveFixture.DbContext);

        Guid bookInfoRequestId = await service.PlaceRequest("Dummy ISBN", CancellationToken.None).ConfigureAwait(false);

        await service.ChangeRequestStatus(bookInfoRequestId, BookInfoRequestStatus.Requested, CancellationToken.None).ConfigureAwait(false);

        List<BookInfoRequestLogEntry> bookInfoRequestLogEntries =
            await _dbDestructiveFixture.DbContext
                .BookInfoRequestLogEntries
                .AsNoTracking()
                .Where(x => x.BookInfoRequestId == bookInfoRequestId)
                .ToListAsync()
                .ConfigureAwait(false);

        bookInfoRequestLogEntries.Should()
            .ContainSingle()
            .Which.Status.Should().Be(BookInfoRequestStatus.Requested);
    }

    [Fact]
    public async Task GivenChangeRequestStatus_WhenMultipleCalls_ThenCorrectEntries()
    {
        BookInfoRequestService service = new(_dbDestructiveFixture.DbContext);

        Guid bookInfoRequestId = await service.PlaceRequest("Dummy ISBN", CancellationToken.None).ConfigureAwait(false);

        await service.ChangeRequestStatus(bookInfoRequestId, "Dummy status 001", CancellationToken.None).ConfigureAwait(false);

        await service.ChangeRequestStatus(bookInfoRequestId, "Dummy status 002", CancellationToken.None).ConfigureAwait(false);

        await service.ChangeRequestStatus(bookInfoRequestId, "Dummy status 003", CancellationToken.None).ConfigureAwait(false);

        await service.ChangeRequestStatus(bookInfoRequestId, "Dummy status 004", CancellationToken.None).ConfigureAwait(false);

        List<BookInfoRequestLogEntry> bookInfoRequestLogEntries =
            await _dbDestructiveFixture.DbContext
                .BookInfoRequestLogEntries
                .AsNoTracking()
                .Where(x => x.BookInfoRequestId == bookInfoRequestId)
                .ToListAsync()
                .ConfigureAwait(false);

        bookInfoRequestLogEntries.Should().HaveCount(5)
            .And.ContainSingle(x => x.Status == BookInfoRequestStatus.Requested)
            .And.ContainSingle(x => x.Status.Equals("Dummy status 004", StringComparison.Ordinal));
    }
}
