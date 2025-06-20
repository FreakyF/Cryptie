using System.Threading;
using System.Threading.Tasks;
using Cryptie.Common.Features.UserManagement.DTOs;

namespace Cryptie.Client.Features.AddFriend.Services;

public interface IFriendsService
{
    Task AddFriendAsync(AddFriendRequestDto addFriendRequest, CancellationToken cancellationToken = default);
}