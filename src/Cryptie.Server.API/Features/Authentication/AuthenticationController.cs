using Cryptie.Server.Domain.Features.Authentication.DTOs;
using Cryptie.Server.Domain.Features.Authentication.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cryptie.Server.API.Features.Authentication;

[ApiController]
[Route("auth")]
public class AuthenticationController(IAuthenticationService authenticationService, IDelayService delayService)
    : ControllerBase
{
    [HttpPost("login", Name = "PostLogin")]
    public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
    {
        return await delayService.FakeDelay(() => authenticationService.LoginHandler(loginRequest));
    }

    [HttpPost("totp", Name = "PostTotp")]
    public async Task<IActionResult> Totp([FromBody] TotpRequest totpRequest)
    {
        return await delayService.FakeDelay(() => authenticationService.TotpHandler(totpRequest));
    }

    [HttpPost("logout", Name = "PostLogout")]
    public async Task<IActionResult> Logout([FromBody] LogoutRequest logoutRequest)
    {
        return await delayService.FakeDelay(() => authenticationService.LogoutHandler(logoutRequest));
    }

    [HttpPost("register", Name = "PostRegister")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest registerRequest)
    {
        return await delayService.FakeDelay(() => authenticationService.RegisterHandler(registerRequest));
    }
}