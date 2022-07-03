using System.Net;
using System.Net.Http.Json;
using Aktabook.Connectors.OpenLibrary.Models;
using Microsoft.Extensions.Logging;

namespace Aktabook.Connectors.OpenLibrary;

public class OpenLibraryClient : IOpenLibraryClient
{
    private readonly string _authorEndpointTemplate = "authors/{0}.json";

    private readonly string _bookEndpointTemplate = "isbn/{0}.json";
    private readonly HttpClient _httpClient;
    private readonly ILogger<OpenLibraryClient> _logger;

    public OpenLibraryClient(HttpClient httpClient,
        ILogger<OpenLibraryClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<Result<Work>> GetBookByIsbnAsync(string isbn,
        CancellationToken cancellationToken)
    {
        Uri requestUri = new(string.Format(_bookEndpointTemplate, isbn),
            UriKind.Relative);

        bool isError = false;
        Work? work = default;
        try
        {
            work =
                await GetRequestInternal<Work>(requestUri, cancellationToken);
        }
        catch (HttpRequestException ex)
            when (ex.StatusCode == HttpStatusCode.NotFound)
        {
        }
        catch (HttpRequestException ex)
        {
            isError = true;
            _logger.LogError(ex, "Open Library work request failed");
        }

        return new Result<Work>
        {
            IsError = isError,
            Value = work
        };
    }

    public async Task<Result<Author>> GetAuthorAsync(string authorId,
        CancellationToken cancellationToken)
    {
        Uri requestUri =
            new(string.Format(_authorEndpointTemplate, authorId),
                UriKind.Relative);

        Author? author = default;
        bool isError = false;
        try
        {
            author =
                await GetRequestInternal<Author>(requestUri, cancellationToken);
            isError = author is null;
        }
        catch (HttpRequestException ex)
        {
            isError = true;
            _logger.LogError(ex, "Open Library author request failed");
        }

        return new Result<Author>
        {
            IsError = isError,
            Value = author
        };
    }

    private async Task<TResponse?> GetRequestInternal<TResponse>(
        Uri requestUri, CancellationToken cancellationToken)
        where TResponse : new()
    {
        return await _httpClient.GetFromJsonAsync<TResponse>(
            requestUri, cancellationToken);
    }
}