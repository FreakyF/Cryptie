using Cryptie.Common.Features.Authentication.DTOs;
using Cryptie.Server.API.Features.Authentication.Validators;
using FluentValidation.TestHelper;

namespace Cryptie.Server.API.Tests.Features.Authentication.Validators;

public class LoginRequestValidatorTests
{
    private readonly LoginRequestValidator _validator;

    public LoginRequestValidatorTests()
    {
        _validator = new LoginRequestValidator();
    }

    [Theory]
    [InlineData(null, "Username cannot be empty")]
    [InlineData("", "Username cannot be empty")]
    [InlineData("     ", "Username cannot be empty")]
    [InlineData("user*&()", "Username contains invalid characters")]
    [InlineData("abcd", "Username must be at least 5 characters long")]
    [InlineData("abcdefghijklmnopqrstuvwxyz", "Username cannot exceed 20 characters")]
    public void InvalidLoginRequest(string username, string ExpectedError)
    {
        //Arrange
        var model = new LoginRequestDto { Login = username, Password = "password!123" };
        //Act
        var result = _validator.TestValidate(model);
        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Login).WithErrorMessage(ExpectedError);
    }

    [Theory]
    [InlineData("User123", "P@ssw0rd123")]
    [InlineData("tester123", "marSJAnin%203")]
    [InlineData("JanGleosia", "Ba1la_Ella")]
    public void ValidLoginRequest(string username, string password)
    {
        //Arrange
        var model = new LoginRequestDto { Login = username, Password = password };
        //Act
        var result = _validator.TestValidate(model);
        //Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Login);
        result.ShouldNotHaveValidationErrorFor(x => x.Password);
    }

    [Theory]
    [InlineData("    ", "Password cannot be empty")]
    [InlineData(null, "Password cannot be empty")]
    [InlineData("", "Password cannot be empty")]
    [InlineData("\u200b", "Password contains invalid characters")]
    [InlineData("Te!st2", "Password must be at least 8 characters long")]
    [InlineData("Test!Passwordabcd12345", "Password cannot exceed 20 characters")]
    [InlineData("password!123", "Password must contain at least one uppercase letter")]
    [InlineData("PASSWORD!123", "Password must contain at least one lowercase letter")]
    [InlineData("PASSWORD!abc", "Password must contain at least one digit")]
    [InlineData("PASSWORD1234abc", "Password must contain at least one special character")]
    public void InvalidPasswordRequest(string password, string ExpectedError)
    {
        //Arrange
        var model = new LoginRequestDto { Login = "User123", Password = password };
        //Act
        var result = _validator.TestValidate(model);
        //Assert
        result.ShouldHaveValidationErrorFor(x => x.Password).WithErrorMessage(ExpectedError);
    }
}