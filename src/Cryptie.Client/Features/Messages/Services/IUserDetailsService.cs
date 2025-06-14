using System.Threading;
using System.Threading.Tasks;
using Cryptie.Common.Features.UserManagement.DTOs;

namespace Cryptie.Client.Features.Messages.Services;

public interface IUserDetailsService
{
    public Task<NameFromGuidResponseDto?> GetUsernameFromGuid(NameFromGuidRequestDto nameFromGuidRequestDto,
        CancellationToken cancellationToken = default);
}