// Copyright (c) 2022 Omar Boukli-Hacene. All rights reserved.
// Distributed under an MIT-style license that can be
// found in the LICENSE file.

// SPDX-License-Identifier: MIT

using FluentValidation.AspNetCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddFluentValidation();
builder.Services.AddHealthChecks();

WebApplication app = builder.Build();

app.UseRouting();
app.UseEndpoints(configure =>
{
    configure.MapControllers();
});
app.UseHealthChecks("/healthz");

app.Run();