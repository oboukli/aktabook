// Copyright (c) Omar Boukli-Hacene. All rights reserved.
// Distributed under an MIT-style license that can be
// found in the LICENSE file.

// SPDX-License-Identifier: MIT

using Aktabook.Application.Messages.Commands;
using Aktabook.Application.Services;
using MediatR;

namespace Aktabook.Application.MessageHandlers;

public class ChangeBookInfoRequestStatusHandler : IRequestHandler<ChangeBookInfoRequestStatus, bool>
{
    private readonly IBookInfoRequester _bookInfoRequester;

    public ChangeBookInfoRequestStatusHandler(IBookInfoRequester bookInfoRequester)
    {
        _bookInfoRequester = bookInfoRequester;
    }

    public async Task<bool> Handle(ChangeBookInfoRequestStatus request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        return await _bookInfoRequester
            .ChangeRequestStatus(request.BookInfoRequestId, request.Status, cancellationToken)
            .ConfigureAwait(false);
    }
}
