using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Cryptie.Server.Features.Authentication.Services;

public class DelayService : IDelayService
{
    private const int TargetMilliseconds = 100;

    /// <summary>
    /// Executes the provided action ensuring that the total execution time is at least
    /// <see cref="TargetMilliseconds"/> milliseconds. This is used to mitigate timing
    /// attacks on authentication endpoints.
    /// </summary>
    /// <param name="func">Action that produces the result.</param>
    /// <returns>The result of <paramref name="func"/> after the enforced delay.</returns>
    public async Task<IActionResult> FakeDelay(Func<IActionResult> func)
    {
        var stopwatch = Stopwatch.StartNew();

        var result = func();

        var delay = TargetMilliseconds - (int)stopwatch.ElapsedMilliseconds;
        if (delay > 0) await Task.Delay(delay);

        return result;
    }
}