// Copyright (c) 2022 Omar Boukli-Hacene. All rights reserved.
// Distributed under an MIT-style license that can be
// found in the LICENSE file.

// SPDX-License-Identifier: MIT

using Aktabook.PublicApi.V1.Dto;
using Aktabook.PublicApi.V1.Validators;
using FluentValidation.TestHelper;
using Xunit;

namespace Aktabook.PublicApi.V1.UnitTest.Validators;

public class CreateBookInfoRequestRequestValidatorUnitTest
{
    [Theory]
    [InlineData("9780199572199")]
    [InlineData("978-0199572199", Skip = "Unsupported")]
    [InlineData("0399184414")]
    [InlineData("978-0399184413", Skip = "Unsupported")]
    public void GivenTestValidate_WhenValidModel_ThenNoValidationErrors(
        string isbn)
    {
        CreateBookInfoRequestRequest model = new() { Isbn = isbn };
        CreateBookInfoRequestRequestValidator validator = new();

        TestValidationResult<CreateBookInfoRequestRequest>? result =
            validator.TestValidate(model);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData("Dummy value")]
    public void GivenTestValidate_WhenInvalidModel_ThenNoValidationErrors(
        string isbn)
    {
        CreateBookInfoRequestRequest model = new() { Isbn = isbn };
        CreateBookInfoRequestRequestValidator validator = new();

        TestValidationResult<CreateBookInfoRequestRequest>? result =
            validator.TestValidate(model);

        result.ShouldHaveAnyValidationError();
    }
}