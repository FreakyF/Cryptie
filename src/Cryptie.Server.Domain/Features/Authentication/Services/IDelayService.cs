using Microsoft.AspNetCore.Mvc;

namespace Cryptie.Server.Domain.Features.Authentication.Services;

public interface IDelayService
{
    Task<IActionResult> FakeDelay(Func<IActionResult> func);
}