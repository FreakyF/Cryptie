using Cryptie.Client.Core.Dependencies;
using Cryptie.Client.Core.Factories;
using Cryptie.Client.Core.Navigation;
using Cryptie.Client.Core.Services;
using Cryptie.Client.Features.Authentication.Services;
using Cryptie.Client.Features.Authentication.State;
using Cryptie.Client.Features.Groups.State;
using Cryptie.Client.Features.Menu.State;
using Cryptie.Common.Features.UserManagement.DTOs;
using Moq;

namespace Cryptie.Client.Tests.Core.Navigation;

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
        var method = typeof(ShellCoordinator).GetMethod("TryInitializeSession", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        object[] parameters = new object[] { null };
        var result = (bool)method.Invoke(_coordinator, parameters);
        var token = (Guid)parameters[0];
        Assert.False(result);
        Assert.Equal(Guid.Empty, token);
    }

    
    [Fact]
    public async Task GetUserGuidAsync_ReturnsGuid()
    {
        var guid = Guid.NewGuid();
        _userDetails.Setup(x => x.GetUserGuidFromTokenAsync(It.IsAny<UserGuidFromTokenRequestDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new UserGuidFromTokenResponseDto { Guid = guid });
        var method = typeof(ShellCoordinator).GetMethod("GetUserGuidAsync", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var task = (Task<Guid>)method.Invoke(_coordinator, new object[] { guid });
        var result = await task;
        Assert.Equal(guid, result);
    }

    [Fact]
    public async Task GetUserGuidAsync_ReturnsEmptyOnNull()
    {
        _userDetails.Setup(x => x.GetUserGuidFromTokenAsync(It.IsAny<UserGuidFromTokenRequestDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserGuidFromTokenResponseDto)null!);
        var method = typeof(ShellCoordinator).GetMethod("GetUserGuidAsync", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var task = (Task<Guid>)method.Invoke(_coordinator, new object[] { Guid.NewGuid() });
        var result = await task;
        Assert.Equal(Guid.Empty, result);
    }

    [Fact]
    public void TryInitializeUser_Fails_ReturnsFalse()
    {
        _keychain.Setup(x => x.TryGetPrivateKey(out It.Ref<string>.IsAny, out It.Ref<string>.IsAny)).Returns(false);
        var method = typeof(ShellCoordinator).GetMethod("TryInitializeUser", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var result = (bool)method.Invoke(_coordinator, new object[] { Guid.NewGuid() });
        Assert.False(result);
    }
    
    private ShellCoordinator CreateCoordinatorWithMocks() => new ShellCoordinator(_factory.Object, _keychain.Object, _userDetails.Object, _connection.Object, _stateDeps);

    
}
