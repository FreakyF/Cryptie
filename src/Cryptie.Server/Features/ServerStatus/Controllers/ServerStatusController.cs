using Cryptie.Server.Features.ServerStatus.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cryptie.Server.Features.ServerStatus.Controllers;

[ApiController]
[Route("status")]
public class ServerStatusController(IServerStatusService serverStatusService) : ControllerBase
{
[HttpPost("server", Name = "GetStatusServer")]
/// <summary>
/// Provides the current server status for monitoring tools.
/// </summary>
/// <returns>HTTP result describing the server health.</returns>
public IActionResult GetServerStatus()
{
    return serverStatusService.GetServerStatus();
}
}