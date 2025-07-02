using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cryptie.Common.Features.GroupManagement;
using Cryptie.Common.Features.GroupManagement.DTOs;

namespace Cryptie.Client.Features.Groups.Services;

public interface IGroupService
{
    Task<Dictionary<Guid, bool>> GetGroupsPrivacyAsync(
        IsGroupsPrivateRequestDto request,
        CancellationToken cancellationToken = default);

    Task<Dictionary<Guid, string>> GetGroupsNamesAsync(
        GetGroupsNamesRequestDto request,
        CancellationToken cancellationToken = default);
}