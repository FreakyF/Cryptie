using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Cryptie.Client.Desktop;

public class AuthApiService(HttpClient httpClient) : IAuthApiService
{
    private readonly HttpClient _httpClient = httpClient;
    public async Task RegisterAsync(RegisterRequest registerRequest)
    {
        var response = await _httpClient.PostAsJsonAsync("auth/register", registerRequest);
        response.EnsureSuccessStatusCode();
    }
}