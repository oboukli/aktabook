using System.Text.Json.Serialization;

namespace Aktabook.Connectors.OpenLibrary.Models;

public class Work
{
    [JsonPropertyName("publishers")]
    public string[]? Publishers { get; set; }

    [JsonPropertyName("number_of_pages")]
    public int NumberOfPages { get; set; }

    [JsonPropertyName("isbn_10")]
    public string[]? Isbn10 { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("isbn_13")]
    public string[]? Isbn13 { get; set; }

    [JsonPropertyName("publish_date")]
    public string? PublishDate { get; set; }
}