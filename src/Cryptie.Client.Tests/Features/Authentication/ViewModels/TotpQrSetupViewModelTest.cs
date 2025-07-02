using System;
using System.Reactive;
using System.Reactive.Threading.Tasks;
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
using MapsterMapper;
using Moq;
using ReactiveUI;
using Xunit;

namespace Cryptie.Client.Tests.Features.Authentication.ViewModels;

// Pomocnicza klasa DTO do testów
internal class RegistrationResponse
{
    public string TotpToken { get; set; }
    public string Secret { get; set; }
} //TODO

public class TotpQrSetupViewModelTest : IDisposable
{
    private readonly Mock<IAuthenticationService> _authMock;
    private readonly Mock<IShellCoordinator> _coordinatorMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IValidator<TotpRequestDto>> _validatorMock;
    private readonly Mock<IExceptionMessageMapper> _exceptionMapperMock;
    private readonly Mock<IRegistrationState> _registrationStateMock;
    private readonly Mock<IScreen> _screenMock;
    private TotpQrSetupViewModel _vm;
    private readonly TotpQrSetupModel _model;
    private readonly TotpRequestDto _dto;
    private readonly RegisterResponseDto _response;

    public TotpQrSetupViewModelTest()
    {
        _authMock = new Mock<IAuthenticationService>();
        _coordinatorMock = new Mock<IShellCoordinator>();
        _mapperMock = new Mock<IMapper>();
        _validatorMock = new Mock<IValidator<TotpRequestDto>>();
        _exceptionMapperMock = new Mock<IExceptionMessageMapper>();
        _registrationStateMock = new Mock<IRegistrationState>();
        _screenMock = new Mock<IScreen>();
        _model = new TotpQrSetupModel { TotpToken = Guid.NewGuid() };
        _dto = new TotpRequestDto { TotpToken = _model.TotpToken, Secret = "secret" };
        _response = new RegisterResponseDto { TotpToken = _model.TotpToken, Secret = "secret" };
        _registrationStateMock.SetupGet(x => x.LastResponse).Returns(_response);
        _mapperMock.Setup(m => m.Map<TotpRequestDto>(It.IsAny<TotpQrSetupModel>())).Returns(_dto);
        _validatorMock.Setup(v => v.ValidateAsync(_dto, It.IsAny<CancellationToken>())).ReturnsAsync(new FluentValidation.Results.ValidationResult());
        _exceptionMapperMock.Setup(x => x.Map(It.IsAny<Exception>())).Returns("error");
        _vm = new TotpQrSetupViewModel(
            _authMock.Object,
            _screenMock.Object,
            _coordinatorMock.Object,
            _validatorMock.Object,
            _exceptionMapperMock.Object,
            _mapperMock.Object,
            _registrationStateMock.Object
        );
    }

    public void Dispose()
    {
    }

    [Fact]
    public void Constructor_SetsPropertiesCorrectly()
    {
        _vm.Model.TotpToken.Should().Be(_response.TotpToken);
        _vm.QrCode.Should().NotBeNullOrEmpty();
        _vm.VerifyCommand.Should().NotBeNull();
    }

    [Fact]
    public void GenerateQrCode_ReturnsSvg()
    {
        var svg = typeof(TotpQrSetupViewModel)
            .GetMethod("GenerateQrCode", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)
            .Invoke(null, new object[] { "payload" }) as string;
        svg.Should().Contain("svg");
    }

    [Fact]
    public async Task VerifyCommand_SuccessfulFlow_CallsShowLogin()
    {
        _authMock.Setup(a => a.TotpAsync(_dto, It.IsAny<CancellationToken>())).ReturnsAsync(new TotpResponseDto { Token = Guid.NewGuid() });
        await _vm.VerifyCommand.Execute().ToTask();
        _coordinatorMock.Verify(c => c.ShowLogin(), Times.Once);
    }

    [Fact]
    public async Task VerifyCommand_AuthReturnsNull_SetsErrorMessage()
    {
        _authMock.Setup(a => a.TotpAsync(_dto, It.IsAny<CancellationToken>())).ReturnsAsync((TotpResponseDto?)null);
        await _vm.VerifyCommand.Execute().ToTask();
        _vm.ErrorMessage.Should().NotBeNullOrEmpty();
    }
    

    [Fact]
    public void VerifyCommand_ThrownException_MapsToErrorMessage()
    {
        _mapperMock.Setup(m => m.Map<TotpRequestDto>(It.IsAny<TotpQrSetupModel>())).Throws(new Exception("fail"));
        _vm = new TotpQrSetupViewModel(
            _authMock.Object,
            _screenMock.Object,
            _coordinatorMock.Object,
            _validatorMock.Object,
            _exceptionMapperMock.Object,
            _mapperMock.Object,
            _registrationStateMock.Object
        );
        _vm.VerifyCommand.Execute().Subscribe(_ => { }, _ => { });
        Task.Delay(100).Wait();
        _vm.ErrorMessage.Should().Be("error");
    }
}
