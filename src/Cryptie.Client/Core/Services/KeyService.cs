using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Cryptie.Common.Features.KeysManagement.DTOs;

namespace Cryptie.Client.Core.Services;

public class KeyService(HttpClient httpClient) : IKeyService
{
    /// <summary>
    ///     Retrieves the RSA public key for the specified user.
    /// </summary>
    /// <param name="getUserKeyRequest">Request containing the user identifier.</param>
    /// <param name="cancellationToken">Token used to cancel the request.</param>
    /// <returns>The user's key information or <c>null</c> when not found.</returns>
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

    /// <summary>
    ///     Retrieves symmetric keys for a collection of groups.
    /// </summary>
    /// <param name="getGroupsKeyRequest">Request specifying groups to retrieve.</param>
    /// <param name="cancellationToken">Token used to cancel the request.</param>
    /// <returns>Response containing keys for the requested groups.</returns>
    public async Task<GetGroupsKeyResponseDto?> GetGroupsKeyAsync(
        GetGroupsKeyRequestDto getGroupsKeyRequest,
        CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "keys/groupsKey");
        request.Content = JsonContent.Create(getGroupsKeyRequest);

        using var response = await httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content
            .ReadFromJsonAsync<GetGroupsKeyResponseDto>(cancellationToken);
    }
}