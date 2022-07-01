// Copyright (c) 2022 Omar Boukli-Hacene. All rights reserved.
// Distributed under an MIT-style license that can be
// found in the LICENSE file.

// SPDX-License-Identifier: MIT

using Aktabook.PublicApi.V1.Validators;
using FluentValidation.AspNetCore;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using Microsoft.OpenApi.Models;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddFluentValidation(x =>
        x.RegisterValidatorsFromAssemblyContaining<
            CreateBookInfoRequestRequestValidator>());

builder.Services.AddHealthChecks();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Aktabook",
        Description = "A book data aggregator API",
        License = new OpenApiLicense
        {
            Name = "MIT",
            Url = new Uri(
                "https://github.com/oboukli/aktabook/blob/main/LICENSE")
        }
    });
});
builder.Services.AddFluentValidationRulesToSwagger();

WebApplication app = builder.Build();

app.UseRouting();
app.UseEndpoints(configure =>
{
    configure.MapControllers();
});
app.UseHealthChecks("/healthz");

app.UseSwagger();
app.UseSwaggerUI();

app.Run();