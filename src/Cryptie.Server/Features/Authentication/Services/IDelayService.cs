using Microsoft.AspNetCore.Mvc;

namespace Cryptie.Server.Features.Authentication.Services;

public interface IDelayService
{
    Task<IActionResult> FakeDelay(Func<IActionResult> func);
}