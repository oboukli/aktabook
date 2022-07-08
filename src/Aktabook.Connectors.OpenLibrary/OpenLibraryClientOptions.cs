// Copyright (c) 2022 Omar Boukli-Hacene. All rights reserved.
// Distributed under an MIT-style license that can be
// found in the LICENSE file.

// SPDX-License-Identifier: MIT

namespace Aktabook.Connectors.OpenLibrary;

public class OpenLibraryClientOptions
{
    public Uri Host { get; set; } = new(string.Empty, UriKind.Relative);
}