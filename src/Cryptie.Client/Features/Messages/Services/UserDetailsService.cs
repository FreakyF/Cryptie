using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Cryptie.Common.Features.UserManagement.DTOs;

namespace Cryptie.Client.Features.Messages.Services;

public class UserDetailsService(HttpClient httpClient) : IUserDetailsService
{
    public async Task<NameFromGuidResponseDto?> GetUsernameFromGuid(NameFromGuidRequestDto nameFromGuidRequestDto,
        CancellationToken cancellationToken = default)
    {
        var url = $"user/namefromguid?Id={nameFromGuidRequestDto.Id}";
        using var response = await httpClient.GetAsync(url, cancellationToken);
        response.EnsureSuccessStatusCode();

        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<NameFromGuidResponseDto>(cancellationToken))!;
    }
}