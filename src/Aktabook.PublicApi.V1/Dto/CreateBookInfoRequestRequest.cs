// Copyright (c) Omar Boukli-Hacene. All rights reserved.
// Distributed under an MIT-style license that can be
// found in the LICENSE file.

// SPDX-License-Identifier: MIT

namespace Aktabook.PublicApi.V1.Dto;

public class CreateBookInfoRequestRequest
{
    public string Isbn { get; set; } = string.Empty;
}
