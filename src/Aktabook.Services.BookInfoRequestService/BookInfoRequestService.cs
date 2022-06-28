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

    public async Task<Guid> PlaceRequest(string isbn,
        CancellationToken cancellationToken)
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

        await _dbContext.BookInfoRequests
            .AddAsync(bookInfoRequest, cancellationToken).ConfigureAwait(false);

        await _dbContext.SaveChangesAsync(cancellationToken)
            .ConfigureAwait(false);

        return bookInfoRequest.BookInfoRequestId;
    }

    public async Task<bool> ChangeRequestStatus(Guid bookInfoRequestId,
        string bookInfoRequestStatus,
        CancellationToken cancellationToken)
    {
        string lastStatus = await _dbContext.BookInfoRequestLogEntries
            .AsNoTracking()
            .Where(x => x.BookInfoRequestId == bookInfoRequestId)
            .GroupBy(x =>
                new
                {
                    x.BookInfoRequestId,
                    x.BookInfoRequestLogEntryId
                })
            .Select(x =>
                new
                {
                    x.Key.BookInfoRequestLogEntryId,
                    Created = x.Max(e => e.Created)
                })
            .Join(_dbContext.BookInfoRequestLogEntries,
                outer => outer.BookInfoRequestLogEntryId,
                inner => inner.BookInfoRequestLogEntryId,
                (q1, q2) => new string(q2.Status)
            )
            .SingleAsync(cancellationToken).ConfigureAwait(false);

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