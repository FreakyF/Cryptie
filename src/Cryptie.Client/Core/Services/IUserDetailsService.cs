using System.Threading;
using System.Threading.Tasks;
using Cryptie.Common.Features.UserManagement.DTOs;

namespace Cryptie.Client.Core.Services;

public interface IUserDetailsService
{
    public Task<NameFromGuidResponseDto?> GetUsernameFromGuidAsync(NameFromGuidRequestDto nameFromGuidRequest,
        CancellationToken cancellationToken = default);

    public Task<UserGuidFromTokenResponseDto?> GetUserGuidFromTokenAsync(
        UserGuidFromTokenRequestDto userGuidFromTokenRequest,
        CancellationToken cancellationToken = default);

    Task<UserPrivateKeyResponseDto?> GetUserPrivateKeyAsync(
        UserPrivateKeyRequestDto userPrivateKeyRequest,
        CancellationToken cancellationToken = default);

    Task<GuidFromLoginResponseDto?> GetGuidFromLoginAsync(
        GuidFromLoginRequestDto guidFromLoginRequest,
        CancellationToken cancellationToken = default);
}