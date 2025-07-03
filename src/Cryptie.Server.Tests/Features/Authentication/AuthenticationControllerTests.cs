using Cryptie.Common.Features.Authentication.DTOs;
using Cryptie.Server.Features.Authentication;
using Cryptie.Server.Features.Authentication.Controllers;
using Cryptie.Server.Features.Authentication.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Cryptie.Server.Tests.Features.Authentication;

public class AuthenticationControllerTests
{
    private readonly Mock<IAuthenticationService> _authenticationServiceMock;
    private readonly Mock<IDelayService> _delayServiceMock;
    private readonly AuthenticationController _controller;

    public AuthenticationControllerTests()
    {
        _authenticationServiceMock = new Mock<IAuthenticationService>();
        _delayServiceMock = new Mock<IDelayService>();
        _controller = new AuthenticationController(_authenticationServiceMock.Object, _delayServiceMock.Object);

        _delayServiceMock
            .Setup(x => x.FakeDelay(It.IsAny<Func<IActionResult>>()))
            .Returns<Func<IActionResult>>(handler => Task.FromResult(handler()));
    }

    [Fact]
    public async Task Login_ShouldCallAuthenticationService_AndReturnItsResult()
    {
        var request = new LoginRequestDto
        {
            Login = "testuser",
            Password = "testpassword"
        };
        var expectedResult = new OkResult();
        
        _authenticationServiceMock
            .Setup(x => x.LoginHandler(request))
            .Returns(expectedResult);

        
        var result = await _controller.Login(request);

        
        result.Should().BeSameAs(expectedResult);
        _authenticationServiceMock.Verify(x => x.LoginHandler(request), Times.Once);
        _delayServiceMock.Verify(x => x.FakeDelay(It.IsAny<Func<IActionResult>>()), Times.Once);
    }

    [Fact]
    public async Task Totp_ShouldCallAuthenticationService_AndReturnItsResult()
    {
        
        var request = new TotpRequestDto
        {
            TotpToken = Guid.NewGuid(),
            Secret = "JBSWY3DPEHPK3PXP" 
        };
        var expectedResult = new OkResult();
        
        _authenticationServiceMock
            .Setup(x => x.TotpHandler(request))
            .Returns(expectedResult);

        
        var result = await _controller.Totp(request);

        
        result.Should().BeSameAs(expectedResult);
        _authenticationServiceMock.Verify(x => x.TotpHandler(request), Times.Once);
        _delayServiceMock.Verify(x => x.FakeDelay(It.IsAny<Func<IActionResult>>()), Times.Once);
    }

    [Fact]
    public async Task Register_ShouldCallAuthenticationService_AndReturnItsResult()
    {
        
        var request = new RegisterRequestDto
        {
            Login = "newuser",
            Password = "newpassword",
            DisplayName = "New User",
            Email = "newuser@example.com",
            PrivateKey = "private-key-data",
            PublicKey = "public-key-data",
            ControlValue = "test-value" // dodano wymagane pole
        };
        var expectedResult = new OkResult();
        
        _authenticationServiceMock
            .Setup(x => x.RegisterHandler(request))
            .Returns(expectedResult);

        
        var result = await _controller.Register(request);

        
        result.Should().BeSameAs(expectedResult);
        _authenticationServiceMock.Verify(x => x.RegisterHandler(request), Times.Once);
        _delayServiceMock.Verify(x => x.FakeDelay(It.IsAny<Func<IActionResult>>()), Times.Once);
    }

    [Fact]
    public async Task DelayService_ShouldBeCalledWithCorrectDelegate()
    {
        
        var request = new LoginRequestDto
        {
            Login = "testuser",
            Password = "testpassword"
        };
        var expectedResult = new OkResult();
        Func<IActionResult> capturedDelegate = null;
        
        _authenticationServiceMock
            .Setup(x => x.LoginHandler(request))
            .Returns(expectedResult);

        _delayServiceMock
            .Setup(x => x.FakeDelay(It.IsAny<Func<IActionResult>>()))
            .Callback<Func<IActionResult>>(d => capturedDelegate = d)
            .Returns(Task.FromResult<IActionResult>(expectedResult));

        
        await _controller.Login(request);

        
        capturedDelegate.Should().NotBeNull();
        var result = capturedDelegate();
        result.Should().BeSameAs(expectedResult);
    }
}
