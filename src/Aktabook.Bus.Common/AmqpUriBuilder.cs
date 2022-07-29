// Copyright (c) 2022 Omar Boukli-Hacene. All rights reserved.
// Distributed under an MIT-style license that can be
// found in the LICENSE file.

// SPDX-License-Identifier: MIT

namespace Aktabook.Bus.Common;

public class AmqpUriBuilder
{
    public string HostName { get; set; } = string.Empty;

    public int PortNumber { get; set; } = 5672;

    public string VirtualHost { get; set; } = string.Empty;

    public string UserName { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public Uri ConnectionUri
    {
        get
        {
            UriBuilder uriBuilder =
                new("amqp", HostName, PortNumber, VirtualHost)
                {
                    Password = Password,
                    UserName = UserName
                };

            return uriBuilder.Uri;
        }
    }

    public override string ToString()
    {
        return ConnectionUri.ToString();
    }
}