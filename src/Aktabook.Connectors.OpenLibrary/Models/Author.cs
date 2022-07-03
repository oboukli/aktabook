using System.Text.Json.Serialization;

namespace Aktabook.Connectors.OpenLibrary.Models;

public class Author
{
    [JsonPropertyName("wikipedia")]
    public Uri? Wikipedia { get; set; }

    [JsonPropertyName("alternate_names")]
    public string[]? AlternateNames { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}