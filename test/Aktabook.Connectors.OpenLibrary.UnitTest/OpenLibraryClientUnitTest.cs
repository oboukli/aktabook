// Copyright (c) 2022 Omar Boukli-Hacene. All rights reserved.
// Distributed under an MIT-style license that can be
// found in the LICENSE file.

// SPDX-License-Identifier: MIT

using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Aktabook.Connectors.OpenLibrary.Models;
using FluentAssertions;
using Xunit;

namespace Aktabook.Connectors.OpenLibrary.UnitTest;

public class OpenLibraryClientUnitTest
{
    [Fact]
    public async Task GivenGetBookByIsbnAsync_WhenIsbnFound_ThenBook()
    {
        HttpClient httpClient = HttpClientMockFactory.CreateHttpClient(HttpStatusCode.OK,
            JsonSerializer.Serialize(new Work()));

        OpenLibraryClient openLibraryClient = new(httpClient);

        Work? result = await openLibraryClient.GetBookByIsbnAsync("Dummy ISBN", CancellationToken.None);

        result.Should().BeOfType<Work>();
    }

    [Fact]
    public async Task GivenGetBookByIsbnAsync_WhenIsbnNotFound_ThenNull()
    {
        HttpClient httpClient = HttpClientMockFactory.CreateHttpClient(HttpStatusCode.NotFound, string.Empty);

        OpenLibraryClient openLibraryClient = new(httpClient);

        Work? result = await openLibraryClient.GetBookByIsbnAsync("Dummy ISBN", CancellationToken.None);

        result.Should().BeNull();
    }

    [Fact]
    public async Task GivenGetBookByIsbnAsync_WhenError_ThenError()
    {
        HttpClient httpClient =
            HttpClientMockFactory.CreateHttpClient(HttpStatusCode.InternalServerError, string.Empty);

        OpenLibraryClient openLibraryClient = new(httpClient);

        await openLibraryClient.Awaiting(x => x.GetBookByIsbnAsync(
                "Dummy ISBN", CancellationToken.None))
            .Should().ThrowExactlyAsync<HttpRequestException>();
    }

    [Fact]
    public async Task GivenGetBookByIsbnAsync_WhenInvalidResponse_ThenException()
    {
        HttpClient httpClient = HttpClientMockFactory.CreateHttpClient(HttpStatusCode.OK, string.Empty);

        OpenLibraryClient openLibraryClient = new(httpClient);

        await openLibraryClient.Awaiting(x => x.GetBookByIsbnAsync("Dummy ISBN",
                CancellationToken.None))
            .Should().ThrowExactlyAsync<JsonException>();
    }

    [Fact]
    public async Task GivenGetAuthorAsync_WhenAuthorId_ThenAuthor()
    {
        HttpClient httpClient = HttpClientMockFactory.CreateHttpClient(HttpStatusCode.OK,
            JsonSerializer.Serialize(new Author()));
        OpenLibraryClient openLibraryClient = new(httpClient);

        Author? result = await openLibraryClient.GetAuthorAsync("Dummy author ID", CancellationToken.None);

        result.Should().BeOfType<Author>();
    }

    [Fact]
    public async Task GivenGetAuthorAsync_WhenAuthorIdNotFound_ThenNull()
    {
        HttpClient httpClient =
            HttpClientMockFactory.CreateHttpClient(HttpStatusCode.NotFound, string.Empty);
        OpenLibraryClient openLibraryClient = new(httpClient);

        Author? result = await openLibraryClient.GetAuthorAsync("Dummy author ID", CancellationToken.None);

        result.Should().BeNull();
    }

    [Fact]
    public async Task GivenGetAuthorAsync_WhenHttpError_ThenThrowHttpRequestException()
    {
        HttpClient httpClient =
            HttpClientMockFactory.CreateHttpClient(HttpStatusCode.InternalServerError, string.Empty);
        OpenLibraryClient openLibraryClient = new(httpClient);

        await openLibraryClient.Awaiting(x =>
                x.GetAuthorAsync("Dummy author ID", CancellationToken.None))
            .Should().ThrowExactlyAsync<HttpRequestException>();
    }

    [Fact]
    public async Task GivenGetAuthorAsync_WhenInvalidResponse_ThenException()
    {
        HttpClient httpClient = HttpClientMockFactory.CreateHttpClient(HttpStatusCode.OK, string.Empty);
        OpenLibraryClient openLibraryClient = new(httpClient);

        await openLibraryClient.Awaiting(x => x.GetAuthorAsync(
                "Dummy author ID", CancellationToken.None)).Should()
            .ThrowExactlyAsync<JsonException>();
    }
}
