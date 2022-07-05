// Copyright (c) 2022 Omar Boukli-Hacene. All rights reserved.
// Distributed under an MIT-style license that can be
// found in the LICENSE file.

// SPDX-License-Identifier: MIT

namespace Aktabook.Connectors.OpenLibrary.Models;

public class Result<T>
{
    public bool IsError { get; set; }

    public T? Value { get; set; }
}