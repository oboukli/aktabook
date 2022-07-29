// Copyright (c) 2022 Omar Boukli-Hacene. All rights reserved.
// Distributed under an MIT-style license that can be
// found in the LICENSE file.

// SPDX-License-Identifier: MIT

using System.Net.Http;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Aktabook.PublicApi.V1.IntegrationTest;

public class TestingWebApplicationFixture<TStartup> where TStartup : class
{
    private readonly WebApplicationFactory<TStartup> _application;

    public TestingWebApplicationFixture()
    {
        _application = new WebApplicationFactory<TStartup>();
    }

    public HttpClient CreateClient()
    {
        return _application.CreateClient();
    }
}