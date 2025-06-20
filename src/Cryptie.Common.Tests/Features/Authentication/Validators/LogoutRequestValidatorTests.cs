﻿using Cryptie.Common.Features.Authentication.DTOs;
using Cryptie.Common.Features.Authentication.Validators;
using FluentValidation.TestHelper;

namespace Cryptie.Common.Tests.Features.Authentication.Validators;

public class LogoutRequestValidatorTests
{
    private readonly LogoutRequestValidator _validator;

    public LogoutRequestValidatorTests()
    {
        _validator = new LogoutRequestValidator();
    }

    [Trait("TestCategory", "Unit")]
    [Fact]
    public void InvalidToken()
    {
        //Arrange
        var model = new LogoutRequestDto { Token = Guid.Empty };
        //Act
        var result = _validator.TestValidate(model);
        //Assret
        result.ShouldHaveValidationErrorFor(x => x.Token);
    }

    [Trait("TestCategory", "Unit")]
    [Fact]
    public void ValidToken()
    {
        //Arrange
        var model = new LogoutRequestDto { Token = Guid.NewGuid() };
        //Act
        var result = _validator.TestValidate(model);
        //Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Token);
    }
}