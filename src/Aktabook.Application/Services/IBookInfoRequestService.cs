// Copyright (c) 2022 Omar Boukli-Hacene. All rights reserved.
// Distributed under an MIT-style license that can be
// found in the LICENSE file.

// SPDX-License-Identifier: MIT

namespace Aktabook.Application.Services;

public interface IBookInfoRequestService
{
    Task<Guid> PlaceRequest(string isbn, CancellationToken cancellationToken);

    Task<bool> ChangeRequestStatus(Guid bookInfoRequestId, string bookInfoRequestStatus,
        CancellationToken cancellationToken);
}
