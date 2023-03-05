// Copyright (c) Omar Boukli-Hacene. All rights reserved.
// Distributed under an MIT-style license that can be
// found in the LICENSE file.

// SPDX-License-Identifier: MIT

using System.Text.Json.Serialization;

namespace Aktabook.Connectors.OpenLibrary.Models;

public class Author
{
    [JsonPropertyName("wikipedia")]
    public Uri? Wikipedia { get; set; }

    [JsonPropertyName("alternate_names")]
    public IReadOnlyCollection<string> AlternateNames { get; init; } = new List<string>();

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}
