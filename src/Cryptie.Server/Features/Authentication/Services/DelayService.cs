using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Cryptie.Server.Features.Authentication.Services;

public class DelayService : IDelayService
{
    private const int TargetMilliseconds = 100;

    /// <summary>
    ///     Executes the given action and ensures that the call takes at least a
    ///     predefined amount of time. Useful to mitigate timing attacks.
    /// </summary>
    /// <param name="func">Action returning an <see cref="IActionResult"/> to execute.</param>
    /// <returns>Result of <paramref name="func"/> after the artificial delay.</returns>
    public async Task<IActionResult> FakeDelay(Func<IActionResult> func)
    {
        var stopwatch = Stopwatch.StartNew();

        var result = func();

        var delay = TargetMilliseconds - (int)stopwatch.ElapsedMilliseconds;
        if (delay > 0) await Task.Delay(delay);

        return result;
    }
}