using Cryptie.Common.Features.Authentication.DTOs;

namespace Cryptie.Client.Application.Features.Authentication.Services;

public interface IAuthenticationService
{
    Task RegisterAsync(RegisterRequestDto registerRequest);
    Task LoginAsync(LoginRequestDto registerRequest);
}