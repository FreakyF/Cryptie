using Cryptie.Common.Features.GroupManagement;
using Cryptie.Common.Features.GroupManagement.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Cryptie.Server.Features.GroupManagement.Services;

public interface IGroupManagementService
{
    public IActionResult IsGroupsPrivate(IsGroupsPrivateRequestDto isGroupsPrivateRequest);
    public IActionResult GetGroupsNames(GetGroupsNamesRequestDto getGroupsNamesRequest);
}