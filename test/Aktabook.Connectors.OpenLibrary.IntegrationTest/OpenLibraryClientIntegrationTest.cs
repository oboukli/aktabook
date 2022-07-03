using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Aktabook.Connectors.OpenLibrary.Models;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Aktabook.Connectors.OpenLibrary.IntegrationTest;

public class OpenLibraryClientIntegrationTest
{
    private static readonly HttpClient HttpClient =
        new() { BaseAddress = new Uri("https://openlibrary.org") };

    private readonly ILogger<OpenLibraryClient> _logger;

    public OpenLibraryClientIntegrationTest()
    {
        ServiceProvider serviceProvider = new ServiceCollection()
            .AddLogging()
            .BuildServiceProvider();

        ILoggerFactory factory =
            serviceProvider.GetRequiredService<ILoggerFactory>();
        _logger = factory.CreateLogger<OpenLibraryClient>();
    }

    [Theory]
    [InlineData("9780140328721")]
    [InlineData("9780199572199")]
    public async Task GivenGetBookByIsbnAsync_WhenIsbnForExistingBook_ThenWork(
        string isbn)
    {
        OpenLibraryClient openLibraryClient = new(HttpClient, _logger);

        Result<Work> result = await openLibraryClient.GetBookByIsbnAsync(isbn,
            CancellationToken.None);

        result.Should().BeEquivalentTo(new
        {
            IsError = false,
            Value = new { }
        }
        );
    }

    [Fact]
    public async Task GivenGetBookByIsbnAsync_WhenInvalidIsbn_ThenValueIsNull()
    {
        OpenLibraryClient openLibraryClient = new(HttpClient, _logger);

        Result<Work> result = await openLibraryClient.GetBookByIsbnAsync("0",
            CancellationToken.None);

        result.Should().BeEquivalentTo(new Result<Work>
        {
            IsError = false,
            Value = null
        }
        );
    }

    [Fact]
    public async Task GivenGetAuthorAsync_WhenAuthorID_ThenAuthor()
    {
        OpenLibraryClient openLibraryClient = new(HttpClient, _logger);

        Result<Author> result = await openLibraryClient.GetAuthorAsync(
            "OL23919A",
            CancellationToken.None);

        result.Should().BeEquivalentTo(new
        {
            IsError = false,
            Value = new { }
        }
        );
    }

    [Fact]
    public async Task GivenGetAuthorAsync_WhenNonExistingAuthorID_ThenError()
    {
        OpenLibraryClient openLibraryClient = new(HttpClient, _logger);

        Result<Author> result = await openLibraryClient.GetAuthorAsync("0",
            CancellationToken.None);

        result.Should().BeEquivalentTo(new { IsError = true }
        );
    }
}