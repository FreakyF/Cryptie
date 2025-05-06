using Cryptie.Common.Features.Authentication.DTOs;
using Cryptie.Common.Features.Authentication.Validators;
using FluentValidation.TestHelper;

namespace Cryptie.Server.API.Tests.Features.Authentication.Validators;

public class RegisterRequestValidatorTests
{
    private readonly RegisterRequestValidator _validator;

    public RegisterRequestValidatorTests()
    {
        _validator = new RegisterRequestValidator();
    }

    [Theory]
    [InlineData(null, "Username cannot be empty")]
    [InlineData("", "Username cannot be empty")]
    [InlineData(" ", "Username cannot be empty")]
    [InlineData(";;;abba", "Username contains invalid characters")]
    [InlineData("ada", "Username must be at least 5 characters long")]
    [InlineData("abcdefghijklmnopqrstuvwxyz", "Username cannot exceed 20 characters")]
    public void InvalidLoginRequest(string username, string ExpectedError)
    {
        //Arrange
        var model = new RegisterRequestDto
            { Login = username, Password = "Password!123", Email = "test@test.pl", DisplayName = "JanMatejko" };
        //Act
        var result = _validator.TestValidate(model);
        //Assert
        result.ShouldHaveValidationErrorFor(x => x.Login).WithErrorMessage(ExpectedError);
    }

    [Theory]
    [InlineData(null, "Display name cannot be empty")]
    [InlineData("", "Display name cannot be empty")]
    [InlineData(" ", "Display name cannot be empty")]
    [InlineData("Abcdefghijklmnopqrstuvwxyz", "Display name cannot exceed 20 characters")]
    public void InvalidDisplayName(string displayName, string ExpectedError)
    {
        //Arrange
        var model = new RegisterRequestDto
        {
            Login = "User1234",
            DisplayName = displayName,
            Password = "Password!123",
            Email = "test@test.pl",
        };
        //Act
        var result = _validator.TestValidate(model);
        //Assert
        result.ShouldHaveValidationErrorFor(x => x.DisplayName).WithErrorMessage(ExpectedError);
    }
    [Theory]
    [InlineData(null, "Password cannot be empty")]
    [InlineData("", "Password cannot be empty")]
    [InlineData(" ", "Password cannot be empty")]
    [InlineData("\u200b12345AA!", "Password contains invalid characters")]
    [InlineData("Te!st2", "Password must be at least 8 characters long")]
    [InlineData("Test!Passwordabcd12345", "Password cannot exceed 20 characters")]
    [InlineData("te!st12356", "Password must contain at least one uppercase letter")]
    [InlineData("TEST!123456", "Password must contain at least one lowercase letter")]
    [InlineData("TEST!test", "Password must contain at least one digit")]
    [InlineData("Test123456", "Password must contain at least one special character")]
    public void InvalidPasswordRequest(string password, string ExpectedError)
    {
        //Arrange
        var model = new RegisterRequestDto
            { Login = "User123", Password = password, Email = "test@test.pl", DisplayName = "JanMatejko" };
        //Act
        var result = _validator.TestValidate(model);
        //Arrange
        result.ShouldHaveValidationErrorFor(x => x.Password).WithErrorMessage(ExpectedError);
    }

    [Theory]
    [InlineData(null, "Email cannot be empty")]
    [InlineData("", "Email cannot be empty")]
    [InlineData(" ", "Email cannot be empty")]
    [InlineData("@test.pl", "Email address is invalid")]
    //[InlineData("mateusz@test,pl", "Email address is invalid")]
    public void InvalidEmailRequest(string email, string ExpectedError)
    {
        //Arrange
        var model = new RegisterRequestDto
            { Login = "User123", Password = "Password!123", Email = email, DisplayName = "JanMatejko" };
        //Act
        var result = _validator.TestValidate(model);
        //Assert
        result.ShouldHaveValidationErrorFor(x => x.Email).WithErrorMessage(ExpectedError);
    }

    [Theory]
    [InlineData("User123", "Password!@1234", "test@test.pl")]
    [InlineData("Mateusz123", "Si34ln&Hasl*", "Mateusz@gmail.com")]
    public void ValidRegisterRequest(string login, string password, string email)
    {
        //Arrange
        var model = new RegisterRequestDto
            { Login = login, Password = password, Email = email, DisplayName = "JanMatejko" };
        //Act
        var result = _validator.TestValidate(model);
        //Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Login);
        result.ShouldNotHaveValidationErrorFor(x => x.Password);
        result.ShouldNotHaveValidationErrorFor(x => x.Email);
    }
}