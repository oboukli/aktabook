// Copyright (c) Omar Boukli-Hacene. All rights reserved.
// Distributed under an MIT-style license that can be
// found in the LICENSE file.

// SPDX-License-Identifier: MIT

using Aktabook.Connectors.OpenLibrary.Models;

namespace Aktabook.Connectors.OpenLibrary;

public interface IOpenLibraryClient
{
    Task<Work?> GetBookByIsbnAsync(string isbn, CancellationToken cancellationToken);

    Task<Author?> GetAuthorAsync(string authorId, CancellationToken cancellationToken);
}
