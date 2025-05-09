using Cryptie.Common.Features.Authentication.DTOs;
using Cryptie.Common.Features.Authentication.Validators;
using FluentValidation.TestHelper;

namespace Cryptie.Server.API.Tests.Features.Authentication.Validators;

public class TotpRequestValidatorTests
{
    private readonly TotpRequestValidator _validator;

    public TotpRequestValidatorTests()
    {
        _validator = new TotpRequestValidator();
    }

    [Trait("TestCategory", "Unit")]
    [Fact]
    public void InvalidTotpRequest()
    {
        //Arrange
        var model = new TotpRequestDto { TotpToken = Guid.Empty, Secret = "123456" };
        //Act
        var result = _validator.TestValidate(model);
        //Assert
        result.ShouldHaveValidationErrorFor(x => x.TotpToken);
    }

    [Trait("TestCategory", "Unit")]
    [Theory]
    [InlineData(null, "Authentication code cannot be empty.")]
    [InlineData("", "Authentication code cannot be empty.")]
    [InlineData(" ", "Authentication code cannot be empty.")]
    [InlineData("123", "Authentication code must be 6 digits long.")]
    [InlineData("1234%6", "Authentication code contains invalid characters.")]
    public void InvalidSecret(string secret, string expectedError)
    {
        //Arrange
        var model = new TotpRequestDto { TotpToken = Guid.NewGuid(), Secret = secret };
        //Act
        var result = _validator.TestValidate(model);
        //Assert
        result.ShouldHaveValidationErrorFor(x => x.Secret);
    }

    [Trait("TestCategory", "Unit")]
    [Fact]
    public void ValidTotpRequest()
    {
        //Arrange
        var model = new TotpRequestDto { TotpToken = Guid.NewGuid(), Secret = "123456" };
        //Act
        var result = _validator.TestValidate(model);
        //Assert
        result.ShouldNotHaveValidationErrorFor(x => x.TotpToken);
        result.ShouldNotHaveValidationErrorFor(x => x.Secret);
    }
}