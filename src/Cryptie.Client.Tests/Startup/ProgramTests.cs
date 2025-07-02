using Avalonia;
using Cryptie.Client.Startup;

namespace Cryptie.Client.Tests.Startup
{
    public class ProgramTests
    {
        [Fact]
        public void BuildAvaloniaApp_ReturnsConfiguredAppBuilder()
        {
            var method = typeof(Program).GetMethod("BuildAvaloniaApp",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            Assert.NotNull(method);
            var builder = method.Invoke(null, null);

            Assert.NotNull(builder);
            Assert.IsType<AppBuilder>(builder);
        }
    }
}