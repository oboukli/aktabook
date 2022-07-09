// Copyright (c) 2022 Omar Boukli-Hacene. All rights reserved.
// Distributed under an MIT-style license that can be
// found in the LICENSE file.

// SPDX-License-Identifier: MIT

using MediatR;

namespace Aktabook.Application.Messages.Commands;

public record PlaceBookInfoRequest(string Isbn) : IRequest<Guid>;