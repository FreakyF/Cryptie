using Cryptie.Client.Infrastructure.Features.Authentication.Services;
using KeySharp;

namespace Cryptie.Client.Infrastructure.Tests.Features.Authentication.Services;

public class KeychainManagerServiceTests
{
    [Trait("TestCategory ","Unit")]
    [Fact]
    public void TrySaveSessionToken_ShouldReturnTrue()
    {
        string token = Guid.NewGuid().ToString();
        var service = new KeychainManagerService();
        string? error;

        var result = service.TrySaveSessionToken(token, out error);
        Assert.True(result);
        Assert.Null(error);
    }
    [Trait("TestCategory ","Unit")]
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void trySaveSessionToken_ShouldReturnFalse(string token)
    {
        var service = new KeychainManagerService();
        string? error;
        
        var result = service.TrySaveSessionToken(token, out error);
        Assert.False(result);
        Assert.Equal("Session token cannot be null or empty.",error);
    }

    [Trait("TestCategory ", "Unit")]
    [Fact]
    public void TryGetSessionToken_ShouldReturnTrue()
    {
        var service = new KeychainManagerService();
        var result = service.TryGetSessionToken(out string? token,out string error);
        
        Assert.True(result);
        Assert.Null(error);
    }

    [Trait("TestCategory ", "Unit")]
    [Fact]
    public void TryGetSessionToken_ShouldReturnFalse()
    {
        var service = new KeychainManagerService();
        service.TryClearSessionToken(out string? errorMessage);
        var result = service.TryGetSessionToken(out string? token, out string error);
        Assert.False(result);
    }

    [Trait("TestCategory ", "Unit")]
    [Fact]
    public void TryClearSessionToken_ShouldReturnTrue()
    {
        string token = Guid.NewGuid().ToString();
        var service = new KeychainManagerService();

        var saveResult = service.TrySaveSessionToken(token, out string? saveError);
        Assert.True(saveResult, $"Save failed: {saveError}");

        var result = service.TryClearSessionToken(out string? clearError);
        Assert.True(result, $"Clear failed: {clearError}");
        Assert.Null(clearError);
    }
}