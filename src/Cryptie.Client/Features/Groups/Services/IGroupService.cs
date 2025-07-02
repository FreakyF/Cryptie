using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cryptie.Common.Features.GroupManagement;
using Cryptie.Common.Features.GroupManagement.DTOs;
using Cryptie.Common.Features.UserManagement.DTOs;

namespace Cryptie.Client.Features.Groups.Services;

public interface IGroupService
{
    Task<IReadOnlyList<Guid>> GetUserGroupsAsync(
        UserGroupsRequestDto request,
        CancellationToken cancellationToken = default);

    Task<Dictionary<Guid, bool>> GetGroupsPrivacyAsync(
        IsGroupsPrivateRequestDto request,
        CancellationToken cancellationToken = default);

    Task<Dictionary<Guid, string>> GetGroupsNamesAsync(
        GetGroupsNamesRequestDto request,
        CancellationToken cancellationToken = default);
}