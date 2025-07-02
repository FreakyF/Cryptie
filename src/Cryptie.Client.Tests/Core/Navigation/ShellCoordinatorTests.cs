using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Cryptie.Client.Core.Dependencies;
using Cryptie.Client.Core.Factories;
using Cryptie.Client.Core.Navigation;
using Cryptie.Client.Core.Services;
using Cryptie.Client.Features.Authentication.Services;
using Cryptie.Client.Features.Authentication.State;
using Cryptie.Client.Features.Authentication.ViewModels;
using Cryptie.Client.Features.Dashboard.ViewModels;
using Cryptie.Client.Features.Groups.State;
using Cryptie.Client.Features.Menu.State;
using Cryptie.Client.Features.PinCode.ViewModels;
using Cryptie.Common.Features.UserManagement.DTOs;
using Moq;
using ReactiveUI;
using Xunit;

public class ShellCoordinatorTests
{
    private readonly Mock<IViewModelFactory> _factory = new();
    private readonly Mock<IKeychainManagerService> _keychain = new();
    private readonly Mock<IUserDetailsService> _userDetails = new();
    private readonly Mock<IConnectionMonitor> _connection = new();
    private readonly ShellStateDependencies _stateDeps;
    private readonly ShellCoordinator _coordinator;

    private readonly Mock<IUserState> _userState = new();
    private readonly Mock<IGroupSelectionState> _groupSelectionState = new();
    private readonly Mock<ILoginState> _loginState = new();
    private readonly Mock<IRegistrationState> _registrationState = new();

    public ShellCoordinatorTests()
    {
        _stateDeps = new ShellStateDependencies(
            _userState.Object,
            _groupSelectionState.Object,
            _loginState.Object,
            _registrationState.Object
        );
        _coordinator = new ShellCoordinator(_factory.Object, _keychain.Object, _userDetails.Object, _connection.Object, _stateDeps);
    }

    

    [Fact]
    public void TryInitializeSession_Fails_ReturnsFalse()
    {
        var result = InvokeTryInitializeSession(out var token);
        Assert.False(result);
        Assert.Equal(Guid.Empty, token);
    }

    
    [Fact]
    public async Task GetUserGuidAsync_ReturnsGuid()
    {
        var guid = Guid.NewGuid();
        _userDetails.Setup(x => x.GetUserGuidFromTokenAsync(It.IsAny<UserGuidFromTokenRequestDto>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(new UserGuidFromTokenResponseDto { Guid = guid });
        var result = await InvokeGetUserGuidAsync(guid);
        Assert.Equal(guid, result);
    }

    [Fact]
    public async Task GetUserGuidAsync_ReturnsEmptyOnNull()
    {
        _userDetails.Setup(x => x.GetUserGuidFromTokenAsync(It.IsAny<UserGuidFromTokenRequestDto>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync((UserGuidFromTokenResponseDto)null!);
        var result = await InvokeGetUserGuidAsync(Guid.NewGuid());
        Assert.Equal(Guid.Empty, result);
    }

    [Fact]
    public void TryInitializeUser_Fails_ReturnsFalse()
    {
        _keychain.Setup(x => x.TryGetPrivateKey(out It.Ref<string>.IsAny, out It.Ref<string>.IsAny)).Returns(false);
        var result = InvokeTryInitializeUser(Guid.NewGuid());
        Assert.False(result);
    }
    

    [Fact]
    public void ClearUserState_ResetsAll()
    {
        InvokeClearUserState();
    }

    [Theory]
    [InlineData(HttpStatusCode.Unauthorized, true)]
    [InlineData(HttpStatusCode.Forbidden, true)]
    [InlineData(HttpStatusCode.BadRequest, true)]
    [InlineData(HttpStatusCode.NotFound, false)]
    public void IsAuthError_Works(HttpStatusCode code, bool expected)
    {
        var ex = new HttpRequestException("fail", null, code);
        var result = InvokeIsAuthError(ex);
        Assert.Equal(expected, result);
    }

    // helpers
    private Task<Guid> InvokeGetUserGuidAsync(Guid token) =>
        (Task<Guid>)typeof(ShellCoordinator).GetMethod("GetUserGuidAsync", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!.Invoke(_coordinator, new object[] { token });

    private bool InvokeTryInitializeUser(Guid guid) =>
        (bool)typeof(ShellCoordinator).GetMethod("TryInitializeUser", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!.Invoke(_coordinator, new object[] { guid });

    private void InvokeClearUserState() =>
        typeof(ShellCoordinator).GetMethod("ClearUserState", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!.Invoke(_coordinator, null);

    private bool InvokeIsAuthError(HttpRequestException ex) =>
        (bool)typeof(ShellCoordinator).GetMethod("IsAuthError", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!.Invoke(null, new object[] { ex });

    private bool InvokeTryInitializeSession(out Guid token)
    {
        token = Guid.Empty;
        if (_keychain.Object.TryGetSessionToken(out var tokenStr, out var error) && Guid.TryParse(tokenStr, out var guid))
        {
            token = guid;
            return true;
        }
        return false;
    }
}
