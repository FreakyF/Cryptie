using Cryptie.Common.Features.Authentication.DTOs;

namespace Cryptie.Common.Tests.Features.Authentication.DTOs;

public class DtosTests
{
    [Trait("TestCategory", "Unit")]
    [Theory]
    [InlineData("login", "password")]
    public void LoginRequestDto_ShouldSetAndReturnCorrectValue(string login, string password)
    {
        var dto = new LoginRequestDto
        {
            Login = login,
            Password = password
        };
        Assert.Equal(login, dto.Login);
        Assert.Equal(password, dto.Password);
    }

    [Trait("TestCategory", "Unit")]
    [Fact]
    public void LoginResponseDto_ShouldSetAndReturnCorrectValue()
    {
        var expectedToken = Guid.NewGuid();
        var dto = new LoginResponseDto
        {
            TotpToken = expectedToken
        };
        Assert.Equal(expectedToken, dto.TotpToken);
    }

    [Trait("TestCategory", "Unit")]
    [Fact]
    public void LogoutRequestDto_ShouldSetAndReturnCorrectValue()
    {
        var expectedToken = Guid.NewGuid();
        var dto = new LogoutRequestDto
        {
            Token = expectedToken
        };
        Assert.Equal(expectedToken, dto.Token);
    }

    [Trait("TestCategory", "Unit")]
    [Theory]
    [InlineData("login", "password", "displayname", "email")]
    public void RegisterRequestDto_ShouldSetAndReturnCorrectValue(string login, string password, string displayName, string email)
    {
        var dto = new RegisterRequestDto
        {
            Login = login,
            Password = password,
            DisplayName = displayName,
            Email = email
        };
        Assert.Equal(login, dto.Login);
        Assert.Equal(password, dto.Password);
        Assert.Equal(displayName, dto.DisplayName);
        Assert.Equal(email, dto.Email);
    }
    
    [Trait("TestCategory", "Unit")]
    [Theory]
    [InlineData("secret")]
    public void RegisterResponseDto_ShouldSetAndReturnCorrectValue(string secret)
    {
        var expectedToken = Guid.NewGuid();
        var dto = new RegisterResponseDto
        {
            TotpToken = expectedToken,
            Secret = secret
        };
        Assert.Equal(expectedToken, dto.TotpToken);
        Assert.Equal(secret, dto.Secret);
    }

    [Trait("TestCategory", "Unit")]
    [Theory]
    [InlineData("secret")]
    public void TotpRequestDto_ShouldSetAndReturnCorrectValue(string secret)
    {
        var expectedToken = Guid.NewGuid();
        var dto = new TotpRequestDto
        {
            TotpToken = expectedToken,
            Secret = secret
        };
        Assert.Equal(expectedToken, dto.TotpToken);
        Assert.Equal(secret, dto.Secret);
    }

    [Trait("TestCategory", "Unit")]
    [Fact]
    public void TotpResponseDto_ShouldSetAndReturnCorrectValue()
    {
        var expectedToken = Guid.NewGuid();
        var dto = new TotpResponseDto { Token = expectedToken };
        Assert.Equal(expectedToken, dto.Token);
    }
}