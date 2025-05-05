using System.Net.Http.Json;
using Cryptie.Client.Application.Features.Authentication.Services;
using Cryptie.Common.Features.Authentication.DTOs;

namespace Cryptie.Client.Infrastructure.Features.Authentication.Services;

public class AuthenticationService(HttpClient httpClient) : IAuthenticationService
{
    public async Task RegisterAsync(RegisterRequestDto registerRequest)
    {
        var response = await httpClient.PostAsJsonAsync("auth/register", registerRequest);
        response.EnsureSuccessStatusCode();
    }

    public async Task LoginAsync(LoginRequestDto registerRequest)
    {
        var response = await httpClient.PostAsJsonAsync("auth/login", registerRequest);
        response.EnsureSuccessStatusCode();
    }
}