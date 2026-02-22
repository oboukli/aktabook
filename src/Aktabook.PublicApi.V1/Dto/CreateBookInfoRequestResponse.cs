// Copyright (c) Omar Boukli-Hacene. All rights reserved.
// Distributed under an MIT-style license that can be
// found in the LICENSE file.

// SPDX-License-Identifier: MIT

namespace Aktabook.PublicApi.V1.Dto;

#pragma warning disable CA1515 // Consider making public types internal
public class CreateBookInfoRequestResponse
#pragma warning restore CA1515 // Consider making public types internal
{
    public Guid BookInfoRequestId { get; set; }
}
