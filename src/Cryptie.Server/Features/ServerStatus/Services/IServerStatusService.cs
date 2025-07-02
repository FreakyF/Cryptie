using Microsoft.AspNetCore.Mvc;

namespace Cryptie.Server.Features.ServerStatus.Services;

public interface IServerStatusService
{
    public IActionResult GetServerStatus();
}