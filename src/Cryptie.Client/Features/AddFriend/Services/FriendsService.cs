using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Cryptie.Common.Features.UserManagement.DTOs;

namespace Cryptie.Client.Features.AddFriend.Services;

public class FriendsService(HttpClient httpClient) : IFriendsService
{
    public async Task AddFriendAsync(AddFriendRequestDto addFriendRequest,
        CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PostAsJsonAsync(
            "user/addfriend",
            addFriendRequest,
            cancellationToken);

        response.EnsureSuccessStatusCode();
    }
}