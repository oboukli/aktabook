// Copyright (c) 2022 Omar Boukli-Hacene. All rights reserved.
// Distributed under an MIT-style license that can be
// found in the LICENSE file.

// SPDX-License-Identifier: MIT

using Microsoft.Extensions.DependencyInjection;

namespace Aktabook.Connectors.OpenLibrary.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IHttpClientBuilder AddOpenLibraryClient(
        this IServiceCollection services, OpenLibraryClientOptions options)
    {
        return services.AddHttpClient<IOpenLibraryClient, OpenLibraryClient>(
            client =>
            {
                client.BaseAddress = options.Host;
            });
    }
}
