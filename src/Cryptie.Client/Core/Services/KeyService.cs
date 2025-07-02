using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Cryptie.Common.Features.KeysManagement.DTOs;

namespace Cryptie.Client.Core.Services;

public class KeyService(HttpClient httpClient) : IKeyService
{
    public async Task<GetUserKeyResponseDto?> GetUserKeyAsync(
        GetUserKeyRequestDto getUserKeyRequest,
        CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "keys/user");
        request.Content = JsonContent.Create(getUserKeyRequest);

        using var response = await httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content
            .ReadFromJsonAsync<GetUserKeyResponseDto>(cancellationToken);
    }
}