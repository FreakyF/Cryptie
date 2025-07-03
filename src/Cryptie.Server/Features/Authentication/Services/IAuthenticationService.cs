using Cryptie.Common.Features.Authentication.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Cryptie.Server.Features.Authentication.Services;

public interface IAuthenticationService
{
    IActionResult LoginHandler(LoginRequestDto loginRequest);

    IActionResult TotpHandler(TotpRequestDto totpRequest);

    IActionResult RegisterHandler(RegisterRequestDto registerRequest);
}