using Cryptie.Server.Features.ServerStatus.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cryptie.Server.Features.ServerStatus;

[ApiController]
[Route("status")]
public class ServerStatusController(IServerStatusService serverStatusService) : ControllerBase
{
    [HttpPost("server", Name = "GetStatusServer")]
    public IActionResult GetServerStatus()
    {
        return serverStatusService.GetServerStatus();
    }
}