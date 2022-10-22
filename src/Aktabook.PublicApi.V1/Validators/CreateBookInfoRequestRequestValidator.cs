// Copyright (c) 2022 Omar Boukli-Hacene. All rights reserved.
// Distributed under an MIT-style license that can be
// found in the LICENSE file.

// SPDX-License-Identifier: MIT

using Aktabook.PublicApi.V1.Dto;
using FluentValidation;

namespace Aktabook.PublicApi.V1.Validators;

public class
    CreateBookInfoRequestRequestValidator : AbstractValidator<
        CreateBookInfoRequestRequest>
{
    public CreateBookInfoRequestRequestValidator()
    {
        RuleFor(x => x.Isbn)
            .MinimumLength(10)
            .MaximumLength(17)
            // Regular expression source: Jan Goyvaerts and Steven Levithan, Regular Expressions Cookbook, 2nd Edition, O'Reilly Media, August 2012
            // https://www.oreilly.com/library/view/regular-expressions-cookbook/9781449327453/ch04s13.html
            .Matches(
                @"^(?=[0-9X]{10}$|(?=(?:[0-9]+[-\ ]){3})[-\ 0-9X]{13}$|97[89][0-9]{10}$|(?=(?:[0-9]+[-\ ]){4})[-\ 0-9]{17}$)(?:97[89][-\ ]?)?[0-9]{1,5}[-\ ]?[0-9]+[-\ ]?[0-9]+[-\ ]?[0-9X]$");
    }
}
