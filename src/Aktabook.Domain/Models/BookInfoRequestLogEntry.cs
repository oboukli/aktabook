// Copyright (c) 2022 Omar Boukli-Hacene. All rights reserved.
// Distributed under an MIT-style license that can be
// found in the LICENSE file.

// SPDX-License-Identifier: MIT

namespace Aktabook.Domain.Models;

public record BookInfoRequestLogEntry(
    Guid BookInfoRequestLogEntryId,
    Guid BookInfoRequestId,
    string Status,
    DateTime Created);