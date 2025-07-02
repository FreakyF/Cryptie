using Microsoft.AspNetCore.Mvc;

namespace Cryptie.Server.Features.ServerStatus.Services;

public class ServerStatusService : ControllerBase, IServerStatusService
{
    public IActionResult GetServerStatus()
    {
        return Ok();
    }
}