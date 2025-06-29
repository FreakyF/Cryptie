using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Cryptie.Common.Features.UserManagement.DTOs;

namespace Cryptie.Client.Features.Account.services;

public class AccountService(HttpClient httpClient) : IAccountService
{
    public async Task ChangeUserDisplayNameAsync(
        UserDisplayNameRequestDto userDisplayNameRequest,
        CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "user/userdisplayname");
        request.Content = JsonContent.Create(userDisplayNameRequest);

        using var response = await httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
    }
}