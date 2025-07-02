using Cryptie.Common.Features.KeysManagement.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Cryptie.Server.Features.KeysManagement.Services;

public interface IKeysManagementService
{
    public IActionResult getUserKey([FromBody] GetUserKeyRequestDto getUserKeyRequest);
    public IActionResult getGroupKey([FromBody] GetGroupKeyRequestDto getGroupKeyRequest);
}