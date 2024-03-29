// Copyright (c) Omar Boukli-Hacene. All rights reserved.
// Distributed under an MIT-style license that can be
// found in the LICENSE file.

// SPDX-License-Identifier: MIT

namespace Aktabook.Domain.Models;

public class BookInfoRequest
{
    public Guid BookInfoRequestId { get; set; }

    public string Isbn { get; set; } = string.Empty;

    public IList<BookInfoRequestLogEntry> BookInfoRequestLogEntries { get; } =
        new List<BookInfoRequestLogEntry>();
}
