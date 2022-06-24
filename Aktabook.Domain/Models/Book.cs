namespace Aktabook.Domain.Models;

public record Book(string Isbn, string Title, IList<Author> Authors);
