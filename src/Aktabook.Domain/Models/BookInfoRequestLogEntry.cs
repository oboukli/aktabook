namespace Aktabook.Domain.Models;

public record BookInfoRequestLogEntry(
    Guid BookInfoRequestLogEntryId,
    Guid BookInfoRequestId,
    string Status,
    DateTime Created);