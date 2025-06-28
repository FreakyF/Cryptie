using Cryptie.Client.Features.Authentication.Mapping;
using Cryptie.Client.Features.Authentication.Models;
using Cryptie.Common.Features.Authentication.DTOs;
using Mapster;
using MapsterMapper;

namespace Cryptie.Client.Tests.Features.Authentication.Mapping;

public class AuthenticationMappingProfileTests
{
    private readonly TypeAdapterConfig _config;

    public AuthenticationMappingProfileTests()
    {
        _config = new TypeAdapterConfig();
        new AuthenticationMappingProfile().Register(_config);
    }

    [Fact]
    public void LoginModel_MapsTo_LoginRequestDto_Correctly()
    {
        // Arrange
        var model = new LoginModel
        {
            Username = "testuser",
            Password = "secret123"
        };

        // Act
        var dto = model.Adapt<LoginRequestDto>(_config);

        // Assert
        Assert.Equal("testuser", dto.Login);
        Assert.Equal("secret123", dto.Password);
    }

    [Fact]
    public void RegisterModel_MapsTo_RegisterRequestDto_Correctly()
    {
        // Arrange
        var model = new RegisterModel
        {
            Username = "newuser",
            Password = "newpass",
            DisplayName = "New User",
            Email = "newuser@example.com"
        };

        // Act
        var dto = model.Adapt<RegisterRequestDto>(_config);

        // Assert
        Assert.Equal("newuser", dto.Login);
        Assert.Equal("newpass", dto.Password);
        Assert.Equal("New User", dto.DisplayName);
        Assert.Equal("newuser@example.com", dto.Email);
    }
}