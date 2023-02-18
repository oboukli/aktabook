// Copyright (c) 2022 Omar Boukli-Hacene. All rights reserved.
// Distributed under an MIT-style license that can be
// found in the LICENSE file.

// SPDX-License-Identifier: MIT

namespace Aktabook.Domain.Models;

public class Author
{
    public Guid AuthorId { get; set; }

    public string Name { get; set; } = string.Empty;

    public IList<Book> Books { get; } = new List<Book>();
}
