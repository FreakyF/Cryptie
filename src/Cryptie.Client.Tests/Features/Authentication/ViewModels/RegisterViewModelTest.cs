using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cryptie.Client.Core.Mapping;
using Cryptie.Client.Core.Navigation;
using Cryptie.Client.Features.Authentication.Models;
using Cryptie.Client.Features.Authentication.Services;
using Cryptie.Client.Features.Authentication.State;
using Cryptie.Client.Features.Authentication.ViewModels;
using Cryptie.Common.Features.Authentication.DTOs;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using MapsterMapper;
using Moq;
using ReactiveUI;
using Xunit;

namespace Cryptie.Client.Tests.Features.Authentication.ViewModels
{
    public class RegisterViewModelTests
    {
        private readonly Mock<IAuthenticationService> _authServiceMock = new();
        private readonly Mock<IShellCoordinator> _coordinatorMock = new();
        private readonly Mock<IValidator<RegisterRequestDto>> _validatorMock = new();
        private readonly Mock<IExceptionMessageMapper> _exceptionMapperMock = new();
        private readonly Mock<IMapper> _mapperMock = new();
        private readonly Mock<IRegistrationState> _registrationStateMock = new();

        private RegisterViewModel CreateViewModel()
        {
            _exceptionMapperMock.Setup(m => m.Map(It.IsAny<Exception>())).Returns("Error");

            _mapperMock.Setup(m => m.Map<RegisterRequestDto>(It.IsAny<RegisterModel>()))
                .Returns(new RegisterRequestDto
                {
                    Login = "testuser",
                    Password = "Password123!",
                    DisplayName = "Test User",
                    Email = "test@example.com",
                    PrivateKey = "",
                    PublicKey = ""
                });

            _validatorMock.Setup(v => v.Validate(It.IsAny<RegisterRequestDto>()))
                .Returns(new ValidationResult());

            _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<RegisterRequestDto>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            return new RegisterViewModel(
                _authServiceMock.Object,
                _coordinatorMock.Object,
                _validatorMock.Object,
                _exceptionMapperMock.Object,
                _mapperMock.Object,
                _registrationStateMock.Object
            );
        }

        [Fact]
        public async Task RegisterCommand_Should_Call_Authentication_And_Navigate_On_Success()
        {
            // Arrange
            var viewModel = CreateViewModel();
            var mockResponse = new RegisterResponseDto { Secret = "secret", TotpToken = Guid.NewGuid() };
            _authServiceMock.Setup(a => a.RegisterAsync(It.IsAny<RegisterRequestDto>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockResponse);

            viewModel.Model.Username = "testuser";
            viewModel.Model.Password = "Password123!";
            viewModel.Model.DisplayName = "Test User";
            viewModel.Model.Email = "test@example.com";

            // Act
            await viewModel.RegisterCommand.Execute();

            // Assert
            _registrationStateMock.VerifySet(r => r.LastResponse = mockResponse, Times.Once);
            _coordinatorMock.Verify(c => c.ShowQrSetup(), Times.Once);
        }

        [Fact]
        public async Task RegisterCommand_Should_Set_ErrorMessage_On_Null_Response()
        {
            // Arrange
            var viewModel = CreateViewModel();
            _authServiceMock.Setup(a => a.RegisterAsync(It.IsAny<RegisterRequestDto>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((RegisterResponseDto?)null);

            viewModel.Model.Username = "testuser";
            viewModel.Model.Password = "Password123!";
            viewModel.Model.DisplayName = "Test User";
            viewModel.Model.Email = "test@example.com";

            // Act
            await viewModel.RegisterCommand.Execute();

            // Assert
            viewModel.ErrorMessage.Should().Be("An error occurred. Please try again.");
        }

        [Fact]
        public void RegisterCommand_Should_Be_Disabled_When_Validation_Fails()
        {
            // Arrange
            var invalidResult = new ValidationResult(new List<ValidationFailure>
            {
                new("Login", "Required")
            });

            _validatorMock.Setup(v => v.Validate(It.IsAny<RegisterRequestDto>())).Returns(invalidResult);

            var viewModel = CreateViewModel();

            viewModel.Model.Username = ""; // Trigger validation

            var canExecute = false;
            viewModel.RegisterCommand.CanExecute.Subscribe(value => canExecute = value);

            // Assert
            canExecute.Should().BeFalse();
        }
    }
}
