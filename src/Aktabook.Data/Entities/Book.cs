// Copyright (c) 2022 Omar Boukli-Hacene. All rights reserved.
// Distributed under an MIT-style license that can be
// found in the LICENSE file.

// SPDX-License-Identifier: MIT

using System.Collections.ObjectModel;

namespace Aktabook.Data.Entities;

public class Book
{
    public Guid BookId { get; set; }

    public string Title { get; set; } = String.Empty;

    public ICollection<Author> Authors { get; set; }
}