using System.Threading;
using System.Threading.Tasks;
using Cryptie.Common.Features.UserManagement.DTOs;

namespace Cryptie.Client.Features.Messages.Services;

public interface IUserDetailsService
{
    public Task<NameFromGuidResponseDto?> GetUsernameFromGuidAsync(NameFromGuidRequestDto nameFromGuidRequest,
        CancellationToken cancellationToken = default);

    public Task<UserGuidFromTokenResponseDto?> GetUserGuidFromTokenAsync(
        UserGuidFromTokenRequestDto userGuidFromTokenRequest,
        CancellationToken cancellationToken = default);
}