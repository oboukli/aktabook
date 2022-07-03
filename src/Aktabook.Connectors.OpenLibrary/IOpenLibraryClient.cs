using Aktabook.Connectors.OpenLibrary.Models;

namespace Aktabook.Connectors.OpenLibrary;

public interface IOpenLibraryClient
{
    Task<Result<Work>> GetBookByIsbnAsync(string isbn,
        CancellationToken cancellationToken);

    Task<Result<Author>> GetAuthorAsync(string authorId,
        CancellationToken cancellationToken);
}