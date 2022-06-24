// Copyright (c) 2022 Omar Boukli-Hacene. All rights reserved.
// Distributed under an MIT-style license that can be
// found in the LICENSE file.

// SPDX-License-Identifier: MIT

using Aktabook.Application.Services;
using MediatR;

namespace Aktabook.Application.Commands;

public class
    ChangeBookInfoRequestStatusHandler : IRequestHandler<
        ChangeBookInfoRequestStatus>
{
    private readonly IBookInfoRequestService _bookInfoRequestService;

    public ChangeBookInfoRequestStatusHandler(
        IBookInfoRequestService bookInfoRequestService)
    {
        _bookInfoRequestService = bookInfoRequestService;
    }

    public async Task<Unit> Handle(ChangeBookInfoRequestStatus request,
        CancellationToken cancellationToken)
    {
        await _bookInfoRequestService.ChangeRequestStatus(
            request.BookInfoRequestId, request.Status, cancellationToken);

        return Unit.Value;
    }
}