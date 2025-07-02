using System.Threading;
using System.Threading.Tasks;
using Cryptie.Common.Features.KeysManagement.DTOs;

namespace Cryptie.Client.Core.Services;

public interface IKeyService
{
    Task<GetUserKeyResponseDto?> GetUserKeyAsync(
        GetUserKeyRequestDto getUserKeyRequest,
        CancellationToken cancellationToken = default);

    Task<GetGroupsKeyResponseDto?> GetGroupsKeyAsync(
        GetGroupsKeyRequestDto getGroupsKeyRequest,
        CancellationToken cancellationToken = default);
}