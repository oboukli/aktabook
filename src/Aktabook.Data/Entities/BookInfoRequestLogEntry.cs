// Copyright (c) 2022 Omar Boukli-Hacene. All rights reserved.
// Distributed under an MIT-style license that can be
// found in the LICENSE file.

// SPDX-License-Identifier: MIT

namespace Aktabook.Data.Entities;

public class BookInfoRequestLogEntry
{
    public Guid BookInfoRequestLogEntryId { get; set; }

    public Guid BookInfoRequestId { get; set; }

    public string Status { get; set; }

    public DateTime Created { get; set; }
}