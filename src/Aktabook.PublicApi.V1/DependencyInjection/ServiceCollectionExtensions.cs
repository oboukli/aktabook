// Copyright (c) 2022 Omar Boukli-Hacene. All rights reserved.
// Distributed under an MIT-style license that can be
// found in the LICENSE file.

// SPDX-License-Identifier: MIT

using Aktabook.Application.MessageHandlers;
using Aktabook.Application.Services;
using Aktabook.Data;
using Aktabook.Services.BookInfoRequestService;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Aktabook.PublicApi.V1.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services,
        Action<DbContextOptionsBuilder> options)
    {
        services.AddDbContext<RequesterServiceDbContext>(options);

        services.AddScoped<IBookInfoRequestService, BookInfoRequestService>();
        services.AddScoped<RequesterServiceDbContext>();

        services.AddMediatR(typeof(PlaceBookInfoRequestHandler).Assembly);

        return services;
    }
}