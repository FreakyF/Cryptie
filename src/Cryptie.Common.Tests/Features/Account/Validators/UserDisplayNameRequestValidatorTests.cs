using Cryptie.Common.Features.Account.Validators;
using Cryptie.Common.Features.UserManagement.DTOs;
using FluentValidation.TestHelper;

namespace Cryptie.Common.Tests.Features.Account.Validators;

public class UserDisplayNameRequestValidatorTests
{
    private readonly UserDisplayNameRequestValidator _validator = new();

    [Theory]
    [InlineData(null, "Username cannot be empty")]
    [InlineData("", "Username cannot be empty")]
    [InlineData("   ", "Username cannot be empty")]
    public void Name_ShouldNotBeNullOrWhiteSpace(string name, string expectedError)
    {
        var dto = new UserDisplayNameRequestDto { Name = name };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Name).WithErrorMessage(expectedError);
    }

    [Theory]
    [InlineData("abc$%", "Username contains invalid characters")]
    [InlineData("abc_123", "Username contains invalid characters")]
    [InlineData("abc-123", "Username contains invalid characters")]
    public void Name_ShouldNotContainInvalidCharacters(string name, string expectedError)
    {
        var dto = new UserDisplayNameRequestDto { Name = name };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Name).WithErrorMessage(expectedError);
    }

    [Theory]
    [InlineData("abcd", "Username must be at least 5 characters long")]
    public void Name_ShouldBeAtLeast5Characters(string name, string expectedError)
    {
        var dto = new UserDisplayNameRequestDto { Name = name };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Name).WithErrorMessage(expectedError);
    }

    [Theory]
    [InlineData("abcdefghijklmnopqrstu", "Username cannot exceed 20 characters")]
    public void Name_ShouldNotExceed20Characters(string name, string expectedError)
    {
        var dto = new UserDisplayNameRequestDto { Name = name };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Name).WithErrorMessage(expectedError);
    }

    [Theory]
    [InlineData("User123")]
    [InlineData("A1B2C3")]
    [InlineData("ValidName20Chars12")]
    public void Name_Valid_ShouldNotHaveValidationError(string name)
    {
        var dto = new UserDisplayNameRequestDto { Name = name };
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
    }
}
