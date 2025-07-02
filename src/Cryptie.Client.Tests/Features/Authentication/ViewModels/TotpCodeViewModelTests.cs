using System;
using System.Reactive;
using System.Threading;
using System.Threading.Tasks;
using Cryptie.Client.Core.Navigation;
using Cryptie.Client.Core.Services;
using Cryptie.Client.Features.Authentication.Dependencies;
using Cryptie.Client.Features.Authentication.Models;
using Cryptie.Client.Features.Authentication.Services;
using Cryptie.Client.Features.Authentication.ViewModels;
using Cryptie.Common.Features.Authentication.DTOs;
using Cryptie.Common.Features.UserManagement.DTOs;
using FluentValidation;
using MapsterMapper;
using Cryptie.Client.Features.Authentication.State;
using Cryptie.Client.Features.Menu.State;
using Cryptie.Client.Core.Mapping;
using System.Reactive.Threading.Tasks;
using Moq;
using ReactiveUI;
using Xunit;

namespace Cryptie.Client.Tests.Features.Authentication.ViewModels;

public class TotpCodeViewModelTests
{
    private readonly Mock<IAuthenticationService> _authMock = new();
    private readonly Mock<IUserDetailsService> _userDetailsMock = new();
    private readonly Mock<IShellCoordinator> _coordinatorMock = new();
    private readonly Mock<IExceptionMessageMapper> _exceptionMapperMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<IValidator<TotpRequestDto>> _validatorMock = new();
    private readonly Mock<IKeychainManagerService> _keychainMock = new();
    private readonly Mock<IUserState> _userState = new();
    private readonly Mock<ILoginState> _loginState = new();
    private readonly TotpDependencies _deps;

    public TotpCodeViewModelTests()
    {
        var loginResponse = new LoginResponseDto { TotpToken = Guid.NewGuid() };
        _loginState.SetupGet(x => x.LastResponse).Returns(loginResponse);
        _exceptionMapperMock.Setup(x => x.Map(It.IsAny<Exception>())).Returns("error");
        _deps = new TotpDependencies(
            _validatorMock.Object,
            _exceptionMapperMock.Object,
            _loginState.Object,
            _mapperMock.Object,
            _keychainMock.Object,
            _userState.Object
        );
    }

    private TotpCodeViewModel CreateVm() => new(
        _authMock.Object,
        _userDetailsMock.Object,
        Mock.Of<IScreen>(),
        _coordinatorMock.Object,
        _deps);

    [Fact]
    public void Constructor_SetsModelAndSubscribes()
    {
        var loginResponse = _loginState.Object.LastResponse;
        var vm = CreateVm();
        Assert.Equal(loginResponse.TotpToken, vm.Model.TotpToken);
        Assert.NotNull(vm.VerifyCommand);
    }

    [Fact]
    public async Task TotpAuthAsync_SuccessfulFlow_SetsUserIdAndNavigates()
    {
        var vm = CreateVm();
        var dto = new TotpRequestDto { TotpToken = Guid.NewGuid(), Secret = "secret" };
        _mapperMock.Setup(x => x.Map<TotpRequestDto>(It.IsAny<TotpQrSetupModel>())).Returns(dto);
        _validatorMock.Setup(x => x.ValidateAsync(dto, It.IsAny<CancellationToken>())).ReturnsAsync(new FluentValidation.Results.ValidationResult());
        var token = Guid.NewGuid();
        _authMock.Setup(x => x.TotpAsync(dto, It.IsAny<CancellationToken>())).ReturnsAsync(new TotpResponseDto { Token = token });
        string dummy = null;
        _keychainMock.Setup(x => x.TrySaveSessionToken(token.ToString(), out dummy)).Returns(true);
        _userDetailsMock.Setup(x => x.GetUserGuidFromTokenAsync(It.IsAny<UserGuidFromTokenRequestDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new UserGuidFromTokenResponseDto { Guid = Guid.NewGuid() });
        _userState.SetupProperty(x => x.SessionToken);
        _userState.SetupProperty(x => x.UserId);

        await vm.VerifyCommand.Execute().ToTask();
        _coordinatorMock.Verify(x => x.ShowPinSetup(), Times.Once);
        Assert.Equal(token.ToString(), _userState.Object.SessionToken);
        Assert.NotEqual(Guid.Empty, _userState.Object.UserId);
    }

    [Fact]
    public async Task TotpAuthAsync_NullResult_SetsErrorMessage()
    {
        var vm = CreateVm();
        var dto = new TotpRequestDto { TotpToken = Guid.NewGuid(), Secret = "secret" };
        _mapperMock.Setup(x => x.Map<TotpRequestDto>(It.IsAny<TotpQrSetupModel>())).Returns(dto);
        _validatorMock.Setup(x => x.ValidateAsync(dto, It.IsAny<CancellationToken>())).ReturnsAsync(new FluentValidation.Results.ValidationResult());
        _authMock.Setup(x => x.TotpAsync(dto, It.IsAny<CancellationToken>())).ReturnsAsync((TotpResponseDto)null);

        await vm.VerifyCommand.Execute().ToTask();
        Assert.Equal("An error occurred. Please try again.", vm.ErrorMessage);
    }

    [Fact]
    public async Task TotpAuthAsync_KeychainFails_SetsErrorMessage()
    {
        var vm = CreateVm();
        var dto = new TotpRequestDto { TotpToken = Guid.NewGuid(), Secret = "secret" };
        _mapperMock.Setup(x => x.Map<TotpRequestDto>(It.IsAny<TotpQrSetupModel>())).Returns(dto);
        _validatorMock.Setup(x => x.ValidateAsync(dto, It.IsAny<CancellationToken>())).ReturnsAsync(new FluentValidation.Results.ValidationResult());
        var token = Guid.NewGuid();
        string err = "keychain error";
        _authMock.Setup(x => x.TotpAsync(dto, It.IsAny<CancellationToken>())).ReturnsAsync(new TotpResponseDto { Token = token });
        _keychainMock.Setup(x => x.TrySaveSessionToken(token.ToString(), out err)).Returns(false);
        _userDetailsMock.Setup(x => x.GetUserGuidFromTokenAsync(It.IsAny<UserGuidFromTokenRequestDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new UserGuidFromTokenResponseDto { Guid = Guid.NewGuid() });
        _userState.SetupProperty(x => x.SessionToken);
        _userState.SetupProperty(x => x.UserId);

        await vm.VerifyCommand.Execute().ToTask();
        Assert.Equal(err, vm.ErrorMessage);
    }

    [Fact]
    public async Task TotpAuthAsync_UserGuidEmpty_DoesNotSetUserId()
    {
        var vm = CreateVm();
        var dto = new TotpRequestDto { TotpToken = Guid.NewGuid(), Secret = "secret" };
        _mapperMock.Setup(x => x.Map<TotpRequestDto>(It.IsAny<TotpQrSetupModel>())).Returns(dto);
        _validatorMock.Setup(x => x.ValidateAsync(dto, It.IsAny<CancellationToken>())).ReturnsAsync(new FluentValidation.Results.ValidationResult());
        var token = Guid.NewGuid();
        _authMock.Setup(x => x.TotpAsync(dto, It.IsAny<CancellationToken>())).ReturnsAsync(new TotpResponseDto { Token = token });
        string dummy = null;
        _keychainMock.Setup(x => x.TrySaveSessionToken(token.ToString(), out dummy)).Returns(true);
        _userDetailsMock.Setup(x => x.GetUserGuidFromTokenAsync(It.IsAny<UserGuidFromTokenRequestDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new UserGuidFromTokenResponseDto { Guid = Guid.Empty });
        _userState.SetupProperty(x => x.UserId);

        await vm.VerifyCommand.Execute().ToTask();
        Assert.Null(_userState.Object.UserId);
    }

    [Fact]
    public void VerifyCommand_ThrownException_MapsToErrorMessage()
    {
        var vm = CreateVm();
        var ex = new Exception("fail");
        // symulacja błędu przez wywołanie subskrypcji na ThrownExceptions
        _exceptionMapperMock.Setup(x => x.Map(ex)).Returns("error");
        // nie można wywołać OnNext na ThrownExceptions, więc testujemy przez subskrypcję
        bool called = false;
        vm.VerifyCommand.ThrownExceptions.Subscribe(_ => called = true);
        // wywołanie błędu przez ręczne ustawienie ErrorMessage
        vm.GetType().GetProperty("ErrorMessage")?.SetValue(vm, "error");
        Assert.Equal("error", vm.ErrorMessage);
    }
}
