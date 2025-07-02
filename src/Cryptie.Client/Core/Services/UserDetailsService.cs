using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Cryptie.Common.Features.UserManagement.DTOs;

namespace Cryptie.Client.Core.Services;

public class UserDetailsService(HttpClient httpClient) : IUserDetailsService
{
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