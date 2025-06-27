using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cryptie.Common.Features.GroupManagement;
using Cryptie.Common.Features.UserManagement.DTOs;

namespace Cryptie.Client.Features.Groups.Services;

public interface IGroupService
{
    Task<IReadOnlyList<Guid>> GetUserGroupsAsync(
        UserGroupsRequestDto request,
        CancellationToken cancellationToken = default);

    Task<string?> GetGroupNameAsync(
        GetGroupNameRequestDto request,
        CancellationToken cancellationToken = default);
}