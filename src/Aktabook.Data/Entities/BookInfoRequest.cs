// Copyright (c) 2022 Omar Boukli-Hacene. All rights reserved.
// Distributed under an MIT-style license that can be
// found in the LICENSE file.

// SPDX-License-Identifier: MIT

namespace Aktabook.Data.Entities;

public class BookInfoRequest
{
    public Guid BookInfoRequestId { get; set; }

    public string Isbn { get; set; }

    public ICollection<BookInfoRequestLogEntry> BookInfoRequestLogEntries { get; set; }
}