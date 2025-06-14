using Microsoft.AspNetCore.Mvc;

namespace Cryptie.Server.Features.ServerStatus;

[ApiController]
[Route("status")]
public class ServerStatusController : ControllerBase
{
    [HttpPost("server", Name = "GetStatusServer")]
    public IActionResult GetServerStatus()
    {
        return Ok();
    }
}