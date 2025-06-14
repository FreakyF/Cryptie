using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Cryptie.Common.Features.UserManagement.DTOs;

namespace Cryptie.Client.Features.Messages.Services;

public class UserDetailsService(HttpClient httpClient) : IUserDetailsService
{
    public async Task<NameFromGuidResponseDto?> GetUsernameFromGuidAsync(
        NameFromGuidRequestDto dto,
        CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "user/namefromguid");
        request.Content = JsonContent.Create(dto);

        using var response = await httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<NameFromGuidResponseDto>(cancellationToken);
    }

    public async Task<UserGuidFromTokenResponseDto?> GetUserGuidFromTokenAsync(
        UserGuidFromTokenRequestDto dto,
        CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "user/guid");
        request.Content = JsonContent.Create(dto);

        using var response = await httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<UserGuidFromTokenResponseDto>(cancellationToken);
    }
}