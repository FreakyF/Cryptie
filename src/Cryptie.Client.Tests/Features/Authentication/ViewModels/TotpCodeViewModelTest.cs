/*using System;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cryptie.Client.Core.Mapping;
using Cryptie.Client.Features.Authentication.ViewModels;
using Cryptie.Client.Features.Authentication.Models;
using Cryptie.Client.Features.Authentication.Services;
using Cryptie.Client.Features.Authentication.Dependencies;
using Cryptie.Common.Features.Authentication.DTOs;
using Cryptie.Common.Features.UserManagement.DTOs;
using Moq;
using Xunit;
using FluentAssertions;
using ReactiveUI;
using Cryptie.Client.Core.Navigation;
using Cryptie.Client.Core.Services;
using Cryptie.Client.Features.Authentication.State;
using Cryptie.Client.Features.Menu.State;
using FluentValidation;
using MapsterMapper;

public class TotpCodeViewModelTests
{
    private readonly Mock<IAuthenticationService> _authService = new();
    private readonly Mock<IUserDetailsService> _userDetailsService = new();
    private readonly Mock<IShellCoordinator> _coordinator = new();
    private readonly Mock<IKeychainManagerService> _keychain = new();
    private readonly Mock<IUserState> _userState = new();
    private readonly Mock<IExceptionMessageMapper> _exceptionMapper = new();
    private readonly Mock<IValidator<TotpRequestDto>> _validator = new();
    private readonly Mock<MapsterMapper.IMapper> _mapper = new();

    private readonly Guid _expectedToken = Guid.NewGuid();
    private readonly Guid _expectedUserId = Guid.NewGuid();


    [Fact]
    public async Task VerifyCommand_ShouldStoreTokenAndNavigate_WhenTotpIsValid()
    {
        // Arrange
        // Mocki dla wszystkich wymaganych zależności
        var validatorMock = new Mock<IValidator<TotpRequestDto>>();
        var exceptionMapperMock = new Mock<IExceptionMessageMapper>();
        var loginStateMock = new Mock<ILoginState>();
        var mapperMock = new Mock<IMapper>();
        var keychainMock = new Mock<IKeychainManagerService>();
        var userStateMock = new Mock<IUserState>();

// Instancja TotpDependencies
        var deps = new TotpDependencies(
            validatorMock.Object,
            exceptionMapperMock.Object,
            loginStateMock.Object,
            mapperMock.Object,
            keychainMock.Object,
            userStateMock.Object
        );
        var tokenResponse = new TotpResponseDto { Token = _expectedToken };

        _authService.Setup(a => a.TotpAsync(It.IsAny<TotpRequestDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(tokenResponse);

        _userDetailsService.Setup(u => u.GetUserGuidFromTokenAsync(
                It.IsAny<UserGuidFromTokenRequestDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new UserGuidFromTokenResponseDto
            {
                Guid = _expectedUserId
            });
        var viewModel = new TotpCodeViewModel(
            _authService.Object,
            _userDetailsService.Object,
            Mock.Of<IScreen>(),
            _coordinator.Object,
            deps);

        viewModel.Model.Secret = "TEST_SECRET";

        // Act
        await viewModel.VerifyCommand.Execute();

        // Assert
        _authService.Verify(a => a.TotpAsync(It.IsAny<TotpRequestDto>(), It.IsAny<CancellationToken>()), Times.Once);
        _userDetailsService.Verify(
            u => u.GetUserGuidFromTokenAsync(It.IsAny<UserGuidFromTokenRequestDto>(), It.IsAny<CancellationToken>()),
            Times.Once);
        _coordinator.Verify(c => c.ShowDashboard(), Times.Once);
        _userState.VerifySet(u => u.SessionToken = _expectedToken.ToString());
        _userState.VerifySet(u => u.UserId = _expectedUserId);
    }

    [Fact]
    public async Task VerifyCommand_ShouldShowError_WhenTotpFails()
    {
        // Arrange
        // Mocki dla wszystkich wymaganych zależności
        var validatorMock = new Mock<IValidator<TotpRequestDto>>();
        var exceptionMapperMock = new Mock<IExceptionMessageMapper>();
        var loginStateMock = new Mock<ILoginState>();
        var mapperMock = new Mock<IMapper>();
        var keychainMock = new Mock<IKeychainManagerService>();
        var userStateMock = new Mock<IUserState>();

// Instancja TotpDependencies
        var deps = new TotpDependencies(
            validatorMock.Object,
            exceptionMapperMock.Object,
            loginStateMock.Object,
            mapperMock.Object,
            keychainMock.Object,
            userStateMock.Object
        );

        _authService.Setup(a => a.TotpAsync(It.IsAny<TotpRequestDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TotpResponseDto)null);

        var viewModel = new TotpCodeViewModel(
            _authService.Object,
            _userDetailsService.Object,
            Mock.Of<IScreen>(),
            _coordinator.Object,
            deps);

        viewModel.Model.Secret = "TEST_SECRET";

        // Act
        await viewModel.VerifyCommand.Execute();

        // Assert
        viewModel.ErrorMessage.Should().Be("An error occurred. Please try again.");
        _coordinator.Verify(c => c.ShowDashboard(), Times.Never);
    }

    [Fact]
    public async Task VerifyCommand_ShouldSetError_WhenKeychainFails()
    {
        // Arrange
        // Mocki dla wszystkich wymaganych zależności
        var validatorMock = new Mock<IValidator<TotpRequestDto>>();
        var exceptionMapperMock = new Mock<IExceptionMessageMapper>();
        var loginStateMock = new Mock<ILoginState>();
        var mapperMock = new Mock<IMapper>();
        var keychainMock = new Mock<IKeychainManagerService>();
        var userStateMock = new Mock<IUserState>();

// Instancja TotpDependencies
        var deps = new TotpDependencies(
            validatorMock.Object,
            exceptionMapperMock.Object,
            loginStateMock.Object,
            mapperMock.Object,
            keychainMock.Object,
            userStateMock.Object
        );
        var err = "Keychain error";

        _authService.Setup(a => a.TotpAsync(It.IsAny<TotpRequestDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TotpResponseDto { Token = _expectedToken });

        _keychain.Setup(k => k.TrySaveSessionToken(It.IsAny<string>(), out err))
            .Returns(false);

        var viewModel = new TotpCodeViewModel(
            _authService.Object,
            _userDetailsService.Object,
            Mock.Of<IScreen>(),
            _coordinator.Object,
            deps);

        viewModel.Model.Secret = "FAKE_SECRET";

        // Act
        await viewModel.VerifyCommand.Execute();

        // Assert
        viewModel.ErrorMessage.Should().Be(err);
        _coordinator.Verify(c => c.ShowDashboard(), Times.Once); // Mimo błędu kontynuuje
    }
}*/