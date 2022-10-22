// Copyright (c) 2022 Omar Boukli-Hacene. All rights reserved.
// Distributed under an MIT-style license that can be
// found in the LICENSE file.

// SPDX-License-Identifier: MIT

using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Aktabook.Connectors.OpenLibrary.Models;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Aktabook.Connectors.OpenLibrary.IntegrationTest;

public class OpenLibraryClientIntegrationTest
{
    private static readonly HttpClient
        HttpClient = new() { BaseAddress = new Uri("https://openlibrary.org") };

    public OpenLibraryClientIntegrationTest()
    {
        ServiceProvider serviceProvider = new ServiceCollection()
            .AddLogging()
            .BuildServiceProvider();
    }

    [Theory]
    [InlineData("9780140328721")]
    [InlineData("9780199572199")]
    public async Task GivenGetBookByIsbnAsync_WhenIsbnForExistingBook_ThenWork(string isbn)
    {
        OpenLibraryClient openLibraryClient = new(HttpClient);

        Work? result = await openLibraryClient.GetBookByIsbnAsync(isbn, CancellationToken.None);

        result.Should().BeOfType<Work>();
    }

    [Fact]
    public async Task GivenGetBookByIsbnAsync_WhenInvalidIsbn_ThenValueIsNull()
    {
        OpenLibraryClient openLibraryClient = new(HttpClient);

        Work? result = await openLibraryClient.GetBookByIsbnAsync("0", CancellationToken.None);

        result.Should().BeNull();
    }

    [Fact]
    public async Task GivenGetAuthorAsync_WhenAuthorId_ThenAuthor()
    {
        OpenLibraryClient openLibraryClient = new(HttpClient);

        Author? result = await openLibraryClient.GetAuthorAsync("OL23919A", CancellationToken.None);

        result.Should().BeOfType<Author>();
    }

    [Fact]
    public async Task GivenGetAuthorAsync_WhenNonExistingAuthorId_ThenValueIsNull()
    {
        OpenLibraryClient openLibraryClient = new(HttpClient);

        Author? result = await openLibraryClient.GetAuthorAsync("0", CancellationToken.None);

        result.Should().BeNull();
    }

    [Fact]
    public void GivenHttpClient_WhenEmptyOpenLibraryClientOptions_ThenArgumentException()
    {
        OpenLibraryClientOptions options = new();

        Action act = () =>
        {
            using HttpClient localHttpClient = new() { BaseAddress = options.Host };
        };

        act.Should().ThrowExactly<ArgumentException>();
    }
}
