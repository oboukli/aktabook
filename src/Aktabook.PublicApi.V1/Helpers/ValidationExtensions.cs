// Copyright (c) 2022 Omar Boukli-Hacene. All rights reserved.
// Distributed under an MIT-style license that can be
// found in the LICENSE file.

// SPDX-License-Identifier: MIT

using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Aktabook.PublicApi.V1.Helpers;

public static class FluentValidationExtensions
{
    public static void AddValidationFailures(
        this ModelStateDictionary modelState,
        ValidationResult validationResult)
    {
        foreach (ValidationFailure error in validationResult.Errors)
        {
            modelState.AddModelError(error.PropertyName,
                error.ErrorMessage);
        }
    }
}