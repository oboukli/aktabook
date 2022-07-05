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
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Aktabook.Connectors.OpenLibrary.UnitTest;

public class OpenLibraryClientUnitTest
{
    private readonly ILogger<OpenLibraryClient> _logger;

    public OpenLibraryClientUnitTest()
    {
        ServiceProvider serviceProvider = new ServiceCollection()
            .AddLogging()
            .BuildServiceProvider();

        ILoggerFactory factory =
            serviceProvider.GetRequiredService<ILoggerFactory>();
        _logger = factory.CreateLogger<OpenLibraryClient>();
    }

    [Fact]
    public async Task GivenGetBookByIsbnAsync_WhenIsbnFound_ThenBook()
    {
        HttpClient httpClient =
            HttpClientMockFactory.CreateHttpClient(HttpStatusCode.OK,
                JsonSerializer.Serialize(new Work()));

        OpenLibraryClient openLibraryClient = new(httpClient, _logger);

        Result<Work> result = await openLibraryClient.GetBookByIsbnAsync(
            "Dummy ISBN",
            CancellationToken.None);

        result.Should().BeEquivalentTo(new Result<Work>
        {
            IsError = false,
            Value = new Work()
        });
    }

    [Fact]
    public async Task GivenGetBookByIsbnAsync_WhenIsbnNotFound_ThenNull()
    {
        HttpClient httpClient =
            HttpClientMockFactory.CreateHttpClient(HttpStatusCode.NotFound,
                string.Empty);

        OpenLibraryClient openLibraryClient = new(httpClient, _logger);

        Result<Work> result = await openLibraryClient.GetBookByIsbnAsync(
            "Dummy ISBN",
            CancellationToken.None);

        result.Should().BeEquivalentTo(new Result<Work>
        {
            IsError = false,
            Value = null
        });
    }

    [Fact]
    public async Task GivenGetBookByIsbnAsync_WhenError_ThenError()
    {
        HttpClient httpClient =
            HttpClientMockFactory.CreateHttpClient(
                HttpStatusCode.InternalServerError,
                string.Empty);

        OpenLibraryClient openLibraryClient = new(httpClient, _logger);

        Result<Work> result = await openLibraryClient.GetBookByIsbnAsync(
            "Dummy ISBN",
            CancellationToken.None);

        result.Should().BeEquivalentTo(new Result<Work>
        {
            IsError = true,
            Value = null
        });
    }

    [Fact]
    public async Task
        GivenGetBookByIsbnAsync_WhenInvalidResponse_ThenException()
    {
        HttpClient httpClient =
            HttpClientMockFactory.CreateHttpClient(HttpStatusCode.OK,
                string.Empty);

        OpenLibraryClient openLibraryClient = new(httpClient, _logger);

        await openLibraryClient.Awaiting(x => x.GetBookByIsbnAsync("Dummy ISBN",
                CancellationToken.None)).Should()
            .ThrowExactlyAsync<JsonException>();
    }

    [Fact]
    public async Task GivenGetAuthorAsync_WhenAuthorID_ThenAuthor()
    {
        HttpClient httpClient =
            HttpClientMockFactory.CreateHttpClient(HttpStatusCode.OK,
                JsonSerializer.Serialize(new Author()));
        OpenLibraryClient openLibraryClient = new(httpClient, _logger);

        Result<Author> result = await openLibraryClient.GetAuthorAsync(
            "Dummy author ID",
            CancellationToken.None);

        result.Should().NotBeEquivalentTo(new Result<Author>
        {
            IsError = true,
            Value = new Author()
        });
    }

    [Fact]
    public async Task GivenGetAuthorAsync_WhenAuthorIDNotFound_ThenError()
    {
        HttpClient httpClient =
            HttpClientMockFactory.CreateHttpClient(HttpStatusCode.NotFound,
                string.Empty);
        OpenLibraryClient openLibraryClient = new(httpClient, _logger);

        Result<Author> result = await openLibraryClient.GetAuthorAsync(
            "Dummy author ID",
            CancellationToken.None);

        result.Should().BeEquivalentTo(new Result<Author>
        {
            IsError = true,
            Value = null
        });
    }

    [Fact]
    public async Task GivenGetAuthorAsync_WhenError_ThenError()
    {
        HttpClient httpClient =
            HttpClientMockFactory.CreateHttpClient(
                HttpStatusCode.InternalServerError,
                string.Empty);
        OpenLibraryClient openLibraryClient = new(httpClient, _logger);

        Result<Author> r = await openLibraryClient.GetAuthorAsync(
            "Dummy author ID",
            CancellationToken.None);

        r.Should().NotBeNull();
    }

    [Fact]
    public async Task GivenGetAuthorAsync_WhenInvalidResponse_ThenException()
    {
        HttpClient httpClient =
            HttpClientMockFactory.CreateHttpClient(HttpStatusCode.OK,
                string.Empty);
        OpenLibraryClient openLibraryClient = new(httpClient, _logger);

        await openLibraryClient.Awaiting(x => x.GetAuthorAsync(
                "Dummy author ID",
                CancellationToken.None)).Should()
            .ThrowExactlyAsync<JsonException>();
    }
}