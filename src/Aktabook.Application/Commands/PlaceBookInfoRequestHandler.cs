// Copyright (c) 2022 Omar Boukli-Hacene. All rights reserved.
// Distributed under an MIT-style license that can be
// found in the LICENSE file.

// SPDX-License-Identifier: MIT

using Aktabook.Application.Services;
using MediatR;

namespace Aktabook.Application.Commands;

public class
    PlaceBookInfoRequestHandler : IRequestHandler<PlaceBookInfoRequest, Guid>
{
    private readonly IBookInfoRequestService _bookInfoRequestService;

    public PlaceBookInfoRequestHandler(
        IBookInfoRequestService bookInfoRequestService)
    {
        _bookInfoRequestService = bookInfoRequestService;
    }

    public async Task<Guid> Handle(PlaceBookInfoRequest request,
        CancellationToken cancellationToken)
    {
        return await _bookInfoRequestService.PlaceRequest(request.Isbn,
            cancellationToken);
    }
}