using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Cryptie.Common.Features.UserManagement.DTOs;

namespace Cryptie.Client.Features.Messages.Services;

public class UserDetailsService(HttpClient httpClient) : IUserDetailsService
{
    public async Task<NameFromGuidResponseDto?> GetUsernameFromGuidAsync(NameFromGuidRequestDto nameFromGuidRequestDto,
        CancellationToken cancellationToken = default)
    {
        var guidEscaped = Uri.EscapeDataString(nameFromGuidRequestDto.Id.ToString());
        var url = $"user/namefromguid?Id={guidEscaped}";
        using var response = await httpClient.GetAsync(url, cancellationToken);
        response.EnsureSuccessStatusCode();

        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<NameFromGuidResponseDto>(cancellationToken))!;
    }

    public async Task<UserGuidFromTokenResponseDto?> GetUserGuidFromTokenAsync(
        UserGuidFromTokenRequestDto userGuidFromTokenRequestDto,
        CancellationToken cancellationToken = default)
    {
        var tokenEscaped = Uri.EscapeDataString(userGuidFromTokenRequestDto.SessionToken.ToString());
        var url = $"user/guid?SessionToken={tokenEscaped}";

        using var response = await httpClient.GetAsync(url, cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<UserGuidFromTokenResponseDto>(cancellationToken);
    }
}