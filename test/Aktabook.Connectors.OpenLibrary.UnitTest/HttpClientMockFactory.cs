// Copyright (c) Omar Boukli-Hacene. All rights reserved.
// Distributed under an MIT-style license that can be
// found in the LICENSE file.

// SPDX-License-Identifier: MIT

using System;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Moq.Protected;

namespace Aktabook.Connectors.OpenLibrary.UnitTest;

public static class HttpClientMockFactory
{
    public static HttpClient CreateHttpClient(HttpStatusCode responseStatusCode, string jsonResponse)
    {
        OpenLibraryClientOptions options = new() { Host = new Uri("https://localhost") };

        return CreateHttpClient(responseStatusCode, jsonResponse, options);
    }

    public static HttpClient CreateHttpClient(HttpStatusCode responseStatusCode, string jsonResponse,
        OpenLibraryClientOptions options)
    {
        return new HttpClient(CreateHttpMessageHandlerMock(responseStatusCode, jsonResponse).Object)
        {
            BaseAddress = options.Host
        };
    }

    private static Mock<HttpMessageHandler> CreateHttpMessageHandlerMock(HttpStatusCode responseStatusCode,
        string jsonResponse)
    {
        Mock<HttpMessageHandler> httpMessageHandlerMock = new(MockBehavior.Strict);
        httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = responseStatusCode,
                Content = new StringContent(jsonResponse, Encoding.UTF8, MediaTypeNames.Application.Json)
            })
            .Verifiable();

        return httpMessageHandlerMock;
    }
}
