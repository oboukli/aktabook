// Copyright (c) 2022 Omar Boukli-Hacene. All rights reserved.
// Distributed under an MIT-style license that can be
// found in the LICENSE file.

// SPDX-License-Identifier: MIT

namespace Aktabook.Domain.Models;

public class Book
{
    public Guid BookId { get; set; }

    public string Title { get; set; } = null!;

    public string Isbn { get; set; }

    public IList<Author> Authors { get; set; } = new List<Author>();
}