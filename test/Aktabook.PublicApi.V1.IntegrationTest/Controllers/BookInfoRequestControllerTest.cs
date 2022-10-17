// Copyright (c) 2022 Omar Boukli-Hacene. All rights reserved.
// Distributed under an MIT-style license that can be
// found in the LICENSE file.

// SPDX-License-Identifier: MIT

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.Mime;
using System.Threading.Tasks;
using Aktabook.Data.Testing.Fixtures;
using Aktabook.Domain.Models;
using Aktabook.PublicApi.V1.Dto;
using FluentAssertions;
using FluentAssertions.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Aktabook.PublicApi.V1.IntegrationTest.Controllers;

public class BookInfoRequestControllerTest :
    IClassFixture<TestingWebApplicationFixture<Program>>,
    IClassFixture<RequesterServiceDbContextSqlServerLongLivedFixture>
{
    private readonly TestingWebApplicationFixture<Program> _app;

    private readonly RequesterServiceDbContextSqlServerLongLivedFixture
        _dbContextFixture;

    public BookInfoRequestControllerTest(
        TestingWebApplicationFixture<Program> app,
        RequesterServiceDbContextSqlServerLongLivedFixture dbContextFixture)
    {
        _app = app;
        _dbContextFixture = dbContextFixture;
    }

    [Fact]
    public async Task GivenGet_WhenNoId_ThenResponseStatusCodeIsNotImplemented()
    {
        HttpClient httpClient = _app.CreateClient();

        HttpResponseMessage response =
            await httpClient.GetAsync("/api/bookinforequest/");

        response.Should().HaveStatusCode(HttpStatusCode.NotImplemented)
            .And.HaveError()
            .And.Subject.Content.Headers.ContentType.Should()
            .BeOfType<MediaTypeHeaderValue>()
            .Which.Should().BeEquivalentTo(
                new MediaTypeHeaderValue("application/problem+json") { CharSet = "utf-8" });
    }

    [Theory]
    [InlineData("/00000000-0000-0000-0000-000000000001")]
    public async Task GivenGet_WhenIncorrectEndpointHandle_ThenRespondWithNotFound(string uri)
    {
        HttpClient httpClient = _app.CreateClient();

        HttpResponseMessage response = await httpClient.GetAsync(uri);

        response.Should().HaveStatusCode(HttpStatusCode.NotFound)
            .And.Subject.Content.Headers.ContentType.Should().BeNull();
    }

    [Theory]
    [InlineData("/api/BookInfoRequest/00000000-0000-0000-0000-000000000001")]
    public async Task GivenGet_WhenCorrectEndpointHandle_ThenResponseStatusCodeIsNotImplemented(string uri)
    {
        HttpClient httpClient = _app.CreateClient();

        HttpResponseMessage response = await httpClient.GetAsync(uri);

        response.Should().HaveStatusCode(HttpStatusCode.NotImplemented)
            .And.HaveError()
            .And.Subject.Content.Headers.ContentType.Should()
            .BeOfType<MediaTypeHeaderValue>()
            .Which.Should().BeEquivalentTo(
                new MediaTypeHeaderValue("application/problem+json") { CharSet = "utf-8" });
    }

    [Theory]
    [InlineData("api/bookinforequest")]
    [InlineData("api/BookInfoRequest")]
    public async Task GivenPostEndpoints_WhenValidRequest_ThenResponseStatusCodeIsAccepted(string uri)
    {
        HttpClient httpClient = _app.CreateClient();

        HttpResponseMessage response = await httpClient.PostAsJsonAsync(
            uri, new CreateBookInfoRequestRequest { Isbn = "9780199572199" });
        CreateBookInfoRequestResponse? result = await response.Content
            .ReadFromJsonAsync<CreateBookInfoRequestResponse>();

        response.Should().HaveStatusCode(HttpStatusCode.Accepted)
            .And.Subject.Content
            .Headers.ContentType.Should().BeOfType<MediaTypeHeaderValue>()
            .Which.Should().BeEquivalentTo(
                new MediaTypeHeaderValue(MediaTypeNames.Application.Json) { CharSet = "utf-8" });
    }

    [Fact]
    public async Task GivenPostEndpoints_WhenValidRequest_ThenResponseIsValid()
    {
        HttpClient httpClient = _app.CreateClient();

        HttpResponseMessage response = await httpClient.PostAsJsonAsync(
            "api/BookInfoRequest",
            new CreateBookInfoRequestRequest { Isbn = "9780199572199" });
        CreateBookInfoRequestResponse? result = await response.Content
            .ReadFromJsonAsync<CreateBookInfoRequestResponse>();

        result.Should().BeOfType<CreateBookInfoRequestResponse>()
            .Which.BookInfoRequestId.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GivenPostEndpoints_WhenValidRequest_ThenDatabaseIsUpdated()
    {
        HttpClient httpClient = _app.CreateClient();

        HttpResponseMessage response = await httpClient.PostAsJsonAsync(
            "api/BookInfoRequest",
            new CreateBookInfoRequestRequest { Isbn = "9780199572199" });

        CreateBookInfoRequestResponse? result = await response.Content
            .ReadFromJsonAsync<CreateBookInfoRequestResponse>();

        BookInfoRequest bookInfoRequest = await _dbContextFixture.DbContext
            .BookInfoRequests
            .AsNoTracking()
            .Include(x => x.BookInfoRequestLogEntries)
            .SingleAsync(x =>
                x.BookInfoRequestId == result!.BookInfoRequestId);

        bookInfoRequest.Should().BeEquivalentTo(
            new BookInfoRequest
            {
                Isbn = "9780199572199",
                BookInfoRequestLogEntries =
                    new List<BookInfoRequestLogEntry> { new() { Status = BookInfoRequestStatus.Requested } }
            }, config =>
                config
                    .Using<Guid>(ctx =>
                        ctx.Subject.Should().NotBeEmpty())
                    .WhenTypeIs<Guid>()
                    .Using<DateTime>(ctx =>
                        ctx.Subject.Should().BeWithin(1.Seconds()))
                    .WhenTypeIs<DateTime>()
                    .Excluding(x =>
                        x.BookInfoRequestLogEntries[0].BookInfoRequest)
        );
    }

    [Fact]
    public async Task GivenPostEndpoints_WhenInvalidRequest_ThenResponseStatusCodeIsBadRequest()
    {
        HttpClient httpClient = _app.CreateClient();

        HttpResponseMessage response = await httpClient.PostAsJsonAsync(
            "api/BookInfoRequest",
            new CreateBookInfoRequestRequest { Isbn = "Invalid ISBN" });
        ValidationProblemDetails? result = await response.Content
            .ReadFromJsonAsync<ValidationProblemDetails>();

        response.Should().HaveStatusCode(HttpStatusCode.BadRequest)
            .And.Subject.Content.Headers.ContentType?.Should()
            .BeOfType<MediaTypeHeaderValue>()
            .Which.Should().BeEquivalentTo(
                new MediaTypeHeaderValue("application/problem+json") { CharSet = "utf-8" });

        result.Should().BeOfType<ValidationProblemDetails>()
            .Which.Errors.Should().HaveCount(1);
    }
}