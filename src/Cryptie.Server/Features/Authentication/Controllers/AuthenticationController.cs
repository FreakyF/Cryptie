using Cryptie.Common.Features.Authentication.DTOs;
using Cryptie.Server.Features.Authentication.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cryptie.Server.Features.Authentication.Controllers;

[ApiController]
[Route("auth")]
public class AuthenticationController(
    IAuthenticationService authenticationService,
    IDelayService delayService
)
    : ControllerBase
{
    [HttpPost("login", Name = "PostLogin")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequest)
    {
        return await delayService.FakeDelay(() => authenticationService.LoginHandler(loginRequest));
    }

    [HttpPost("totp", Name = "PostTotp")]
    public async Task<IActionResult> Totp([FromBody] TotpRequestDto totpRequest)
    {
        return await delayService.FakeDelay(() => authenticationService.TotpHandler(totpRequest));
    }

    [HttpPost("register", Name = "PostRegister")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto registerRequest)
    {
        return await delayService.FakeDelay(() => authenticationService.RegisterHandler(registerRequest));
    }
}