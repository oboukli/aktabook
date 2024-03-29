// Copyright (c) Omar Boukli-Hacene. All rights reserved.
// Distributed under an MIT-style license that can be
// found in the LICENSE file.

// SPDX-License-Identifier: MIT

using MediatR;

namespace Aktabook.Application.Messages.Commands;

public record ChangeBookInfoRequestStatus(Guid BookInfoRequestId, string Status) : IRequest<bool>;
