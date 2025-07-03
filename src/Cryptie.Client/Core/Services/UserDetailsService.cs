using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Cryptie.Common.Features.UserManagement.DTOs;

namespace Cryptie.Client.Core.Services;

public class UserDetailsService(HttpClient httpClient) : IUserDetailsService
{
    /// <summary>
    ///     Retrieves a user's display name based on their GUID.
    /// </summary>
    /// <param name="nameFromGuidRequest">Request containing the user GUID.</param>
    /// <param name="cancellationToken">Token used to cancel the request.</param>
    /// <returns>The user's display name or <c>null</c> when not found.</returns>
    public async Task<NameFromGuidResponseDto?> GetUsernameFromGuidAsync(
        NameFromGuidRequestDto nameFromGuidRequest,
        CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "user/namefromguid");
        request.Content = JsonContent.Create(nameFromGuidRequest);

        using var response = await httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<NameFromGuidResponseDto>(cancellationToken);
    }

    /// <summary>
    ///     Gets the user's GUID associated with a session token.
    /// </summary>
    public async Task<UserGuidFromTokenResponseDto?> GetUserGuidFromTokenAsync(
        UserGuidFromTokenRequestDto userGuidFromTokenRequest,
        CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "user/guid");
        request.Content = JsonContent.Create(userGuidFromTokenRequest);

        using var response = await httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<UserGuidFromTokenResponseDto>(cancellationToken);
    }

    /// <summary>
    ///     Retrieves the private key for the specified user.
    /// </summary>
    public async Task<UserPrivateKeyResponseDto?> GetUserPrivateKeyAsync(
        UserPrivateKeyRequestDto userPrivateKeyRequest,
        CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "user/privateKey");
        request.Content = JsonContent.Create(userPrivateKeyRequest);

        using var response = await httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<UserPrivateKeyResponseDto>(cancellationToken);
    }


    /// <summary>
    ///     Resolves a user's GUID from their login name.
    /// </summary>
    public async Task<GuidFromLoginResponseDto?> GetGuidFromLoginAsync(
        GuidFromLoginRequestDto guidFromLoginRequest,
        CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "user/guidfromlogin");
        request.Content = JsonContent.Create(guidFromLoginRequest);

        using var response = await httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content
            .ReadFromJsonAsync<GuidFromLoginResponseDto>(cancellationToken);
    }
}