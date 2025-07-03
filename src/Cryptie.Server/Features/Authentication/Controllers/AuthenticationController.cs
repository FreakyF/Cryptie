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
/// <summary>
/// Initiates the login process for a user.
/// </summary>
/// <param name="loginRequest">Credentials provided by the client.</param>
/// <returns>A task returning the authentication result.</returns>
public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequest)
{ 
    return await delayService.FakeDelay(() => authenticationService.LoginHandler(loginRequest));
}

[HttpPost("totp", Name = "PostTotp")]
/// <summary>
/// Validates a TOTP code and returns a session token if successful.
/// </summary>
/// <param name="totpRequest">TOTP verification payload.</param>
/// <returns>A task returning the result of the verification.</returns>
public async Task<IActionResult> Totp([FromBody] TotpRequestDto totpRequest)
{ 
    return await delayService.FakeDelay(() => authenticationService.TotpHandler(totpRequest));
}

[HttpPost("register", Name = "PostRegister")]
/// <summary>
/// Registers a new user in the system.
/// </summary>
/// <param name="registerRequest">User details and credentials.</param>
/// <returns>A task describing the outcome of the registration.</returns>
public async Task<IActionResult> Register([FromBody] RegisterRequestDto registerRequest)
{ 
    return await delayService.FakeDelay(() => authenticationService.RegisterHandler(registerRequest));
}
}