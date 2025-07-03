using System;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using Cryptie.Client.Core.Mapping;
using Cryptie.Client.Core.Navigation;
using Cryptie.Client.Features.Authentication.Models;
using Cryptie.Client.Features.Authentication.Services;
using Cryptie.Client.Features.Authentication.State;
using Cryptie.Client.Features.Authentication.ViewModels;
using Cryptie.Client.Features.Menu.State;
using Cryptie.Common.Features.Authentication.DTOs;
using FluentValidation;
using FluentValidation.Results;
using MapsterMapper;
using Moq;
using Xunit;

namespace Cryptie.Client.Tests.Features.Authentication.ViewModels;

public class LoginViewModelTests
{
    private readonly Mock<IAuthenticationService> _authMock = new();
    private readonly Mock<IShellCoordinator> _coordinatorMock = new();
    private readonly Mock<IValidator<LoginRequestDto>> _validatorMock = new();
    private readonly Mock<IExceptionMessageMapper> _exceptionMapperMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<ILoginState> _loginStateMock = new();
    private readonly Mock<IUserState> _userStateMock = new();

    private LoginViewModel CreateViewModel() =>
        new LoginViewModel(
            _authMock.Object,
            _coordinatorMock.Object,
            _validatorMock.Object,
            _exceptionMapperMock.Object,
            _userStateMock.Object, // Zmieniono z null na mock IUserState
            _mapperMock.Object,
            _loginStateMock.Object);

    [Fact]
    public async Task LoginAsync_InvalidValidation_ShouldSetErrorMessage()
    {
        // Arrange
        var viewModel = CreateViewModel();
        viewModel.Model.Username = "user";
        viewModel.Model.Password = "pass";

        var dto = new LoginRequestDto { Login = "user", Password = "pass" };
        _mapperMock.Setup(m => m.Map<LoginRequestDto>(It.IsAny<LoginModel>())).Returns(dto);
        _validatorMock.Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(new[] { new ValidationFailure("Login", "error") }));

        // Act
        await viewModel.LoginCommand.Execute();

        // Assert
        Assert.Equal("Wrong username or password.", viewModel.ErrorMessage);
        _authMock.Verify(a => a.LoginAsync(It.IsAny<LoginRequestDto>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task LoginAsync_NullResponse_ShouldSetErrorMessage()
    {
        // Arrange
        var viewModel = CreateViewModel();
        viewModel.Model.Username = "user";
        viewModel.Model.Password = "pass";

        var dto = new LoginRequestDto { Login = "user", Password = "pass" };

        _mapperMock.Setup(m => m.Map<LoginRequestDto>(It.IsAny<LoginModel>())).Returns(dto);
        _validatorMock.Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _authMock.Setup(a => a.LoginAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync((LoginResponseDto?)null);

        // Act
        await viewModel.LoginCommand.Execute();

        // Assert
        Assert.Equal("Wrong username or password.", viewModel.ErrorMessage);
        _coordinatorMock.Verify(c => c.ShowTotpCode(), Times.Never);
    }

    [Fact]
    public async Task LoginAsync_SuccessfulLogin_ShouldNavigateAndSaveState()
    {
        // Arrange
        var viewModel = CreateViewModel();
        viewModel.Model.Username = "user";
        viewModel.Model.Password = "pass";

        var dto = new LoginRequestDto { Login = "user", Password = "pass" };
        var response = new LoginResponseDto { TotpToken = Guid.NewGuid() };

        _mapperMock.Setup(m => m.Map<LoginRequestDto>(It.IsAny<LoginModel>())).Returns(dto);
        _validatorMock.Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _authMock.Setup(a => a.LoginAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        await viewModel.LoginCommand.Execute();

        // Assert
        _loginStateMock.VerifySet(s => s.LastResponse = response, Times.Once);
        _coordinatorMock.Verify(c => c.ShowTotpCode(), Times.Once);
    }

    [Fact]
    public async Task LoginCommand_ThrowsException_ShouldMapErrorMessage()
    {
        // Arrange
        var viewModel = CreateViewModel();
        var exception = new Exception("Something failed");
        _mapperMock.Setup(m => m.Map<LoginRequestDto>(It.IsAny<LoginModel>())).Throws(exception);
        _exceptionMapperMock.Setup(m => m.Map(exception)).Returns("Mapped error");

        // Act
        var thrown = false;
        viewModel.LoginCommand.ThrownExceptions
            .Subscribe(_ => thrown = true);

        await Assert.ThrowsAsync<Exception>(() => viewModel.LoginCommand.Execute().ToTask());

        // Assert
        Assert.True(thrown);
    }
}
