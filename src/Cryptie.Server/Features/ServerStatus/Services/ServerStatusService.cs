using Microsoft.AspNetCore.Mvc;

namespace Cryptie.Server.Features.ServerStatus.Services;

public class ServerStatusService : ControllerBase, IServerStatusService
{
    /// <summary>
    ///     Simple health-check endpoint.
    /// </summary>
    /// <returns><see cref="OkResult"/> when the server is running.</returns>
    public IActionResult GetServerStatus()
    {
        return Ok();
    }
}