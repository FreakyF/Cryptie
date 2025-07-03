using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Cryptie.Common.Features.Authentication.DTOs;

namespace Cryptie.Client.Features.Authentication.Services;

public class AuthenticationService(HttpClient httpClient) : IAuthenticationService
{
    /// <summary>
    ///     Sends a registration request to the backend.
    /// </summary>
    /// <param name="registerRequest">User registration data.</param>
    /// <param name="cancellationToken">Token used to cancel the request.</param>
    /// <returns>Registration result or <c>null</c> on failure.</returns>
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

    /// <summary>
    ///     Logs a user into the system.
    /// </summary>
    public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto loginRequest,
        CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PostAsJsonAsync(
            "auth/login",
            loginRequest,
            cancellationToken);

        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<LoginResponseDto>(cancellationToken))!;
    }

    /// <summary>
    ///     Performs the second factor TOTP verification.
    /// </summary>
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