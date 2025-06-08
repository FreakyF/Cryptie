using Cryptie.Common.Features.Authentication.DTOs;

namespace Cryptie.Client.Domain.Features.Authentication.Services;

public interface IAuthenticationService
{
    Task<RegisterResponseDto?> RegisterAsync(RegisterRequestDto registerRequest,
        CancellationToken cancellationToken = default);

    Task<LoginResponseDto?> LoginAsync(LoginRequestDto loginRequest, CancellationToken cancellationToken = default);
    Task<TotpResponseDto?> TotpAsync(TotpRequestDto totpRequest, CancellationToken cancellationToken = default);
}