// Copyright (c) 2022 Omar Boukli-Hacene. All rights reserved.
// Distributed under an MIT-style license that can be
// found in the LICENSE file.

// SPDX-License-Identifier: MIT

namespace Aktabook.Domain.Models;

public class BookInfoRequest
{
    public Guid RequestId { get; set; }

    public string Isbn { get; set; } = null!;

    public IList<BookInfoRequestLogEntry> BookInfoRequestLogEntries
    {
        get;
        set;
    } = new List<BookInfoRequestLogEntry>();
}