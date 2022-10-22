// Copyright (c) 2022 Omar Boukli-Hacene. All rights reserved.
// Distributed under an MIT-style license that can be
// found in the LICENSE file.

// SPDX-License-Identifier: MIT

namespace Aktabook.Bus.Common;

public class AmqpUriBuilder
{
    public string HostName { get; set; } = string.Empty;

    public int PortNumber { get; set; }

    public string VirtualHost { get; set; } = string.Empty;

    public string UserName { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public Uri ConnectionUri
    {
        get
        {
            UriBuilder uriBuilder =
                new("amqp", Uri.EscapeDataString(HostName), PortNumber,
                    Uri.EscapeDataString(VirtualHost))
                {
                    Password = Uri.EscapeDataString(Password),
                    UserName = Uri.EscapeDataString(UserName)
                };

            return uriBuilder.Uri;
        }
    }
}
