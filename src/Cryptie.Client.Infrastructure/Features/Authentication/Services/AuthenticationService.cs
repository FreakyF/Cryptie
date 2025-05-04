using System.Net.Http.Json;
using Cryptie.Client.Application;
using Cryptie.Client.Application.Features.Authentication.Services;

namespace Cryptie.Client.Infrastructure.Features.Authentication.Services;

public class AuthenticationService(HttpClient httpClient) : IAuthenticationService
{
    public async Task RegisterAsync(RegisterRequest registerRequest)
    {
        var response = await httpClient.PostAsJsonAsync("auth/register", registerRequest);
        response.EnsureSuccessStatusCode();
    }
}