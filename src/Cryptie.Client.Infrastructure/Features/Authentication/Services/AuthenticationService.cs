using System.Net.Http.Json;
using Cryptie.Client.Domain.Features.Authentication.Services;
using Cryptie.Common.Features.Authentication.DTOs;

namespace Cryptie.Client.Infrastructure.Features.Authentication.Services;

public class AuthenticationService(HttpClient httpClient) : IAuthenticationService
{
    public async Task RegisterAsync(RegisterRequestDto registerRequest, CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PostAsJsonAsync(
            "auth/register",
            registerRequest,
            cancellationToken);

        response.EnsureSuccessStatusCode();
    }

    public async Task LoginAsync(LoginRequestDto loginRequest, CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PostAsJsonAsync(
            "auth/login",
            loginRequest,
            cancellationToken);

        response.EnsureSuccessStatusCode();
    }
}