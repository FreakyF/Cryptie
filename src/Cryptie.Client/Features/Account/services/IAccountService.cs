using System.Threading;
using System.Threading.Tasks;
using Cryptie.Common.Features.UserManagement.DTOs;

namespace Cryptie.Client.Features.Account.services;

public interface IAccountService
{
    Task ChangeUserDisplayNameAsync(
        UserDisplayNameRequestDto userDisplayNameRequest,
        CancellationToken cancellationToken = default);
}