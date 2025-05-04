using Cryptie.Server.API.Features.Authentication.Validators;
using Cryptie.Server.Domain.Features.Authentication.DTOs;
using FluentValidation.TestHelper;

namespace Cryptie.Server.API.Tests.Features.Authentication.Validators;

public class TotpRequestValidatorTests
{
    private readonly TotpRequestValidator _validator;

    public TotpRequestValidatorTests()
    {
        _validator = new TotpRequestValidator();
    }

    [Fact]
    public void InvalidTotpRequest()
    {
        //Arrange
        var model = new TotpRequest { TotpToken = Guid.Empty, Secret = "123456" };
        //Act
        var result = _validator.TestValidate(model);
        //Assert
        result.ShouldHaveValidationErrorFor(x => x.TotpToken);
    }

    [Theory]
    [InlineData(null, "Authentication code cannot be empty.")]
    [InlineData("", "Authentication code cannot be empty.")]
    [InlineData(" ", "Authentication code cannot be empty.")]
    [InlineData("123", "Authentication code must be 6 digits long.")]
    [InlineData("1234%6", "Authentication code contains invalid characters.")]
    public void InvalidSecret(string secret, string expectedError)
    {
        //Arrange
        var model = new TotpRequest { TotpToken = Guid.NewGuid(), Secret = secret };
        //Act
        var result = _validator.TestValidate(model);
        //Assert
        result.ShouldHaveValidationErrorFor(x => x.Secret);
    }

    [Fact]
    public void ValidTotpRequest()
    {
        //Arrange
        var model = new TotpRequest { TotpToken = Guid.NewGuid(), Secret = "123456" };
        //Act
        var result = _validator.TestValidate(model);
        //Assert
        result.ShouldNotHaveValidationErrorFor(x => x.TotpToken);
        result.ShouldNotHaveValidationErrorFor(x => x.Secret);
    }
}