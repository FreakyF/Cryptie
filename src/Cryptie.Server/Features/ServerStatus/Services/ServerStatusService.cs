using Microsoft.AspNetCore.Mvc;

namespace Cryptie.Server.Features.ServerStatus.Services;

public class ServerStatusService : ControllerBase, IServerStatusService
{
    /// <summary>
    /// Simple health check endpoint confirming that the server is running.
    /// </summary>
    /// <returns>HTTP 200 response.</returns>
    public IActionResult GetServerStatus()
    {
        return Ok();
    }
}