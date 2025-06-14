using Microsoft.AspNetCore.Mvc;

namespace Cryptie.Common.Features.Authentication.Services;

public interface IDelayService
{
    Task<IActionResult> FakeDelay(Func<IActionResult> func);
}