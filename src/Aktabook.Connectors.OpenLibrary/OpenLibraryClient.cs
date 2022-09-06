// Copyright (c) 2022 Omar Boukli-Hacene. All rights reserved.
// Distributed under an MIT-style license that can be
// found in the LICENSE file.

// SPDX-License-Identifier: MIT

using System.Net;
using System.Net.Http.Json;
using Aktabook.Connectors.OpenLibrary.Models;

namespace Aktabook.Connectors.OpenLibrary;

public class OpenLibraryClient : IOpenLibraryClient
{
    private readonly string _authorEndpointTemplate = "authors/{0}.json";

    private readonly string _bookEndpointTemplate = "isbn/{0}.json";
    private readonly HttpClient _httpClient;

    public OpenLibraryClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<Work?> GetBookByIsbnAsync(string isbn, CancellationToken cancellationToken)
    {
        Uri requestUri = new(string.Format(_bookEndpointTemplate, isbn), UriKind.Relative);

        try
        {
            return await GetRequestInternal<Work>(requestUri, cancellationToken).ConfigureAwait(false);
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public async Task<Author?> GetAuthorAsync(string authorId, CancellationToken cancellationToken)
    {
        Uri requestUri = new(string.Format(_authorEndpointTemplate, authorId), UriKind.Relative);

        try
        {
            return await GetRequestInternal<Author>(requestUri, cancellationToken).ConfigureAwait(false);
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    private async Task<TResponse?> GetRequestInternal<TResponse>(Uri requestUri,
        CancellationToken cancellationToken)
        where TResponse : new()
    {
        return await _httpClient.GetFromJsonAsync<TResponse>(requestUri, cancellationToken)
            .ConfigureAwait(false);
    }
}