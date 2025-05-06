using Cryptie.Common.Features.Authentication.DTOs;

namespace Cryptie.Client.Domain.Features.Authentication.Services;

public interface IAuthenticationService
{
    Task RegisterAsync(RegisterRequestDto registerRequest, CancellationToken cancellationToken = default);
    Task LoginAsync(LoginRequestDto loginRequest, CancellationToken cancellationToken = default);
}