using Cryptie.Server.API.Features.Authentication.Validators;
using Cryptie.Server.Domain.Features.Authentication.DTOs;
using FluentValidation.TestHelper;
using Microsoft.AspNetCore.Identity;

namespace Cryptie.Server.API.Tests.Features.Authentication.Validators;

public class LogoutRequestValidatorTests
{
    private readonly LogoutRequestValidator _validator;

    public LogoutRequestValidatorTests()
    {
        _validator = new LogoutRequestValidator();
    }

    [Fact]
    public void InvalidToken()
    {
        //Arrange
        var model = new LogoutRequest { Token = Guid.Empty };
        //Act
        var result = _validator.TestValidate(model);
        //Assret
        result.ShouldHaveValidationErrorFor(x => x.Token);
    }

    [Fact]
    public void ValidToken()
    {
        //Arrange
        var model = new LogoutRequest { Token = Guid.NewGuid() };
        //Act
        var result = _validator.TestValidate(model);
        //Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Token);
    }

}