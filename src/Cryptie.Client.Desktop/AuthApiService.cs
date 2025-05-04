using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Cryptie.Client.Desktop;

public class AuthApiService(HttpClient httpClient) : IAuthApiService
{
    public async Task RegisterAsync(RegisterRequest registerRequest)
    {
        var response = await httpClient.PostAsJsonAsync("auth/register", registerRequest);
        response.EnsureSuccessStatusCode();
    }
}