using Cryptie.Client.Features.Authentication.Services;

namespace Cryptie.Client.Tests.Features.Authentication.Services;

public class IKeychainManagerServiceTests : IClassFixture<KeychainManagerServiceFixture>
{
    private static IKeychainManagerService _service;

    public IKeychainManagerServiceTests(KeychainManagerServiceFixture fixture)
    {
        _service = fixture.Service;
    }

    [Trait("TestCategory", "Unit")]
    [Fact]
    public void TrySaveSessionTokenReturnsTrue()
    {
        var token = Guid.NewGuid().ToString();
        string? error;

        var result = _service.TrySaveSessionToken(token, out error);

        Assert.True(result);
        Assert.Null(error);
    }

    [Trait("TestCategory", "Unit")]
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void TrySaveSessionTokenReturnsFalse(string token)
    {
        string? error;
        var result = _service.TrySaveSessionToken(token, out error);


        Assert.False(result);
        Assert.Equal("Session token cannot be null or empty.", error);
    }

    [Trait("TestCategory", "Unit")]
    [Fact]
    public void TryGetSessionTokenReturnsTrue()
    {
        var token = Guid.NewGuid().ToString();
        _service.TrySaveSessionToken(token, out var error);
        var result = _service.TryGetSessionToken(out var gettoken, out error);

        Assert.True(result);
        Assert.Equal(token, gettoken);
        Assert.Null(error);
    }

    [Trait("TestCategory", "Unit")]
    [Fact]
    public void TryGetSessionTokenReturnsFalse()
    {
        _service.TryClearSessionToken(out var _);
        var result = _service.TryGetSessionToken(out var _, out _);
        Assert.False(result);
    }
}