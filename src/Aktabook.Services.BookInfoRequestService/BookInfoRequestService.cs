// Copyright (c) 2022 Omar Boukli-Hacene. All rights reserved.
// Distributed under an MIT-style license that can be
// found in the LICENSE file.

// SPDX-License-Identifier: MIT

using Aktabook.Application.Services;
using Aktabook.Data;
using Aktabook.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Aktabook.Services.BookInfoRequestService;

public class BookInfoRequestService : IBookInfoRequestService
{
    private readonly RequesterServiceDbContext _dbContext;

    public BookInfoRequestService(RequesterServiceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Guid> PlaceRequest(string isbn, CancellationToken cancellationToken)
    {
        BookInfoRequest bookInfoRequest = new()
        {
            Isbn = isbn,
            BookInfoRequestLogEntries = new List<BookInfoRequestLogEntry>
            {
                new()
                {
                    Status = BookInfoRequestStatus.Requested,
                    Created = DateTime.UtcNow
                }
            }
        };

        await _dbContext.BookInfoRequests.AddAsync(bookInfoRequest, cancellationToken).ConfigureAwait(false);

        await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return bookInfoRequest.BookInfoRequestId;
    }

    /// <summary>
    ///     Change request status in the underlying database.
    /// </summary>
    /// <remarks>
    ///     The method makes an effort to avoid consecutively repeating status
    ///     changes, however it does not guarantee in thar regard.
    /// </remarks>
    /// <param name="bookInfoRequestId"></param>
    /// <param name="bookInfoRequestStatus"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>
    ///     <c>true</c> if operation completed; <c>false</c> if operation
    ///     skipped in order to avoid a repetition.
    /// </returns>
    public async Task<bool> ChangeRequestStatus(Guid bookInfoRequestId,
        string bookInfoRequestStatus,
        CancellationToken cancellationToken)
    {
        string lastStatus = await _dbContext.BookInfoRequestLogEntries
            .AsNoTracking()
            .Where(x => x.BookInfoRequestId == bookInfoRequestId)
            .OrderByDescending(x => x.Created)
            .Select(x => x.Status)
            .FirstAsync(cancellationToken).ConfigureAwait(false);

        if (lastStatus.Equals(bookInfoRequestStatus, StringComparison.Ordinal))
        {
            return false;
        }

        EntityEntry<BookInfoRequestLogEntry> entry =
            _dbContext.Entry(new BookInfoRequestLogEntry
            {
                BookInfoRequestId = bookInfoRequestId,
                Status = bookInfoRequestStatus,
                Created = DateTime.UtcNow
            });
        entry.State = EntityState.Added;

        await _dbContext.SaveChangesAsync(cancellationToken)
            .ConfigureAwait(false);

        return true;
    }
}