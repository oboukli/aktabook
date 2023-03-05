// Copyright (c) Omar Boukli-Hacene. All rights reserved.
// Distributed under an MIT-style license that can be
// found in the LICENSE file.

// SPDX-License-Identifier: MIT

using Aktabook.Application.Messages.Commands;
using Aktabook.Application.Messages.Events;

namespace Aktabook.Bus.Common;

public class BusMessageConvention : IMessageConvention
{
    public string Name { get; } = "Aktabook message convention";

    public bool IsCommandType(Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        return type.Namespace == typeof(PlaceBookInfoRequest).Namespace;
    }

    public bool IsEventType(Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        return type.Namespace == typeof(BookInfoRequestProcessed).Namespace;
    }

    public bool IsMessageType(Type type)
    {
        return IsCommandType(type) || IsEventType(type);
    }
}
