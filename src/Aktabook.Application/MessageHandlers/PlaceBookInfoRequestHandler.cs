// Copyright (c) Omar Boukli-Hacene. All rights reserved.
// Distributed under an MIT-style license that can be
// found in the LICENSE file.

// SPDX-License-Identifier: MIT

using Aktabook.Application.Messages.Commands;
using Aktabook.Application.Services;
using MediatR;

namespace Aktabook.Application.MessageHandlers;

public class PlaceBookInfoRequestHandler : IRequestHandler<PlaceBookInfoRequest, Guid>
{
    private readonly IBookInfoRequestService _bookInfoRequestService;
    private readonly IMessageSession _messageSession;

    public PlaceBookInfoRequestHandler(IBookInfoRequestService bookInfoRequestService,
        IMessageSession messageSession)
    {
        _bookInfoRequestService = bookInfoRequestService;
        _messageSession = messageSession;
    }

    public async Task<Guid> Handle(PlaceBookInfoRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        Guid bookInfoRequestId = await _bookInfoRequestService.PlaceRequest(request.Isbn, cancellationToken)
            .ConfigureAwait(false);

        await _messageSession
            .Send(new ProcessBookInfoRequest(bookInfoRequestId, request.Isbn), cancellationToken)
            .ConfigureAwait(false);

        return bookInfoRequestId;
    }
}
