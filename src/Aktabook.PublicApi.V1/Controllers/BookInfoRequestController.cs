// Copyright (c) Omar Boukli-Hacene. All rights reserved.
// Distributed under an MIT-style license that can be
// found in the LICENSE file.

// SPDX-License-Identifier: MIT

using Aktabook.Application.Messages.Commands;
using Aktabook.PublicApi.V1.Dto;
using Aktabook.PublicApi.V1.Helpers;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Aktabook.PublicApi.V1.Controllers;

[ApiController]
[ApiConventionType(typeof(DefaultApiConventions))]
[Route("api/[controller]")]
public class BookInfoRequestController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IValidator<CreateBookInfoRequestRequest> _validator;

    public BookInfoRequestController(IMediator mediator, IValidator<CreateBookInfoRequestRequest> validator)
    {
        _mediator = mediator;
        _validator = validator;
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
    [ProducesResponseType(typeof(CreateBookInfoRequestResponse), StatusCodes.Status202Accepted)]
    public async Task<ActionResult<CreateBookInfoRequestResponse>> Post(
        [FromBody]
        CreateBookInfoRequestRequest createBookInfoRequestRequest)
    {
        ValidationResult validationResult =
            await _validator.ValidateAsync(createBookInfoRequestRequest).ConfigureAwait(false);

        if (!validationResult.IsValid)
        {
            ModelState.AddValidationFailures(validationResult);

            return ValidationProblem();
        }

        PlaceBookInfoRequest placeBookInfoRequest = new(createBookInfoRequestRequest.Isbn);

        Guid bookInfoRequestId = await _mediator.Send(placeBookInfoRequest).ConfigureAwait(false);

        CreateBookInfoRequestResponse response = new() { BookInfoRequestId = bookInfoRequestId };

        return AcceptedAtAction(nameof(Get), response.BookInfoRequestId, response);
    }
}
