using System.Net.Http.Json;
using Cryptie.Client.Domain.Features.Authentication.Services;
using Cryptie.Common.Features.Authentication.DTOs;

namespace Cryptie.Client.Infrastructure.Features.Authentication.Services;

public class AuthenticationService(HttpClient httpClient) : IAuthenticationService
{
    public async Task<RegisterResponseDto?> RegisterAsync(RegisterRequestDto registerRequest,
        CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PostAsJsonAsync(
            "auth/register",
            registerRequest,
            cancellationToken);

        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<RegisterResponseDto>(cancellationToken))!;
    }

    public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto loginRequest, CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PostAsJsonAsync(
            "auth/login",
            loginRequest,
            cancellationToken);

        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<LoginResponseDto>(cancellationToken))!;
    }

    public async Task<TotpResponseDto?> TotpAsync(TotpRequestDto totpRequest,
        CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PostAsJsonAsync(
            "auth/totp",
            totpRequest,
            cancellationToken);

        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<TotpResponseDto>(cancellationToken))!;
    }
}