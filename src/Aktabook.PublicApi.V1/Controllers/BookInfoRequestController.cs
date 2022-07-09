// Copyright (c) 2022 Omar Boukli-Hacene. All rights reserved.
// Distributed under an MIT-style license that can be
// found in the LICENSE file.

// SPDX-License-Identifier: MIT

using Aktabook.Application.Messages.Commands;
using Aktabook.PublicApi.V1.Dto;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Aktabook.PublicApi.V1.Controllers;

[ApiController]
[ApiConventionType(typeof(DefaultApiConventions))]
[Route("api/[controller]")]
public class BookInfoRequestController : ControllerBase
{
    private readonly IMediator _mediator;

    public BookInfoRequestController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status501NotImplemented)]
    public ActionResult<IEnumerable<object>> Get()
    {
        return StatusCode(StatusCodes.Status501NotImplemented);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status501NotImplemented)]
    public ActionResult<object> Get(Guid id)
    {
        return StatusCode(StatusCodes.Status501NotImplemented);
    }

    [HttpPost]
    public async Task<ActionResult<CreateBookInfoRequestResponse>> Post(
        [FromBody]
        CreateBookInfoRequestRequest createBookInfoRequestRequest)
    {
        PlaceBookInfoRequest placeBookInfoRequest =
            new(createBookInfoRequestRequest.Isbn);

        Guid bookInfoRequestId = await _mediator.Send(placeBookInfoRequest);

        CreateBookInfoRequestResponse response = new()
        {
            BookInfoRequestId = bookInfoRequestId
        };

        return CreatedAtAction(nameof(Get), response.BookInfoRequestId,
            response);
    }
}