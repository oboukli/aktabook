// Copyright (c) Omar Boukli-Hacene. All rights reserved.
// Distributed under an MIT-style license that can be
// found in the LICENSE file.

// SPDX-License-Identifier: MIT

using Aktabook.Application.MessageHandlers;
using Aktabook.Application.Services;
using Aktabook.Data;
using Aktabook.Services.BookInfoRequestService;
using Microsoft.EntityFrameworkCore;

namespace Aktabook.PublicApi.V1.DependencyInjection;

internal static class ServiceCollectionExtensions
{
    internal static IServiceCollection AddApplicationServices(this IServiceCollection services,
        Action<DbContextOptionsBuilder> options)
    {
        services.AddDbContext<RequesterServiceDbContext>(options);

        services.AddScoped<IBookInfoRequester, BookInfoRequester>();
        services.AddScoped<RequesterServiceDbContext>();

        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssemblyContaining<PlaceBookInfoRequestHandler>()
        );

        return services;
    }
}
