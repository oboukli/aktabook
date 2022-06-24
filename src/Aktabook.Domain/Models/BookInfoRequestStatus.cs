// Copyright (c) 2022 Omar Boukli-Hacene. All rights reserved.
// Distributed under an MIT-style license that can be
// found in the LICENSE file.

// SPDX-License-Identifier: MIT

namespace Aktabook.Domain.Models;

public sealed class BookInfoRequestStatus
{
    public const string Requested = "requested";

    public const string InProgress = "in_progress";

    public const string Fulfilled = "fulfilled";

    public const string Failed = "failed";

    public const string Rejected = "rejected";
}