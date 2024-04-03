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
        return new HttpClient(CreateHttpMessageHandlerMock(responseStatusCode, jsonResponse))
        {
            BaseAddress = options.Host
        };
    }

    private static HttpMessageHandler CreateHttpMessageHandlerMock(HttpStatusCode responseStatusCode, string jsonResponse)
    {
        HttpMessageHandler httpMessageHandlerMock = new HttpMessageHandlerMock(responseStatusCode, jsonResponse);

        return httpMessageHandlerMock;
    }

    private sealed class HttpMessageHandlerMock : HttpMessageHandler
    {
        private readonly HttpStatusCode _responseStatusCode;
        private readonly string _jsonResponse;

        public HttpMessageHandlerMock(HttpStatusCode responseStatusCode, string jsonResponse)
        {
            _responseStatusCode = responseStatusCode;
            _jsonResponse = jsonResponse;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new HttpResponseMessage
            {
                StatusCode = _responseStatusCode,
                Content = new StringContent(_jsonResponse, Encoding.UTF8, MediaTypeNames.Application.Json)
            });
        }
    }
}
