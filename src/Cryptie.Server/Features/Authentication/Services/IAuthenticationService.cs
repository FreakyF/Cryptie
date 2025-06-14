using Cryptie.Common.Features.Authentication.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Cryptie.Common.Features.Authentication.Services;

public interface IAuthenticationService
{
    IActionResult LoginHandler(LoginRequestDto loginRequest);

    IActionResult TotpHandler(TotpRequestDto totpRequest);

    IActionResult LogoutHandler(LogoutRequestDto logoutRequest);

    IActionResult RegisterHandler(RegisterRequestDto registerRequest);
}