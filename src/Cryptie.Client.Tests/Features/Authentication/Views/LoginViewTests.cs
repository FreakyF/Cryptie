using Cryptie.Client.Features.Authentication.Views;
using Xunit;

namespace Cryptie.Client.Tests.Features.Authentication.Views;

public class LoginViewTests
{
    [Fact(Skip = "ReactiveUserControl wymaga środowiska Avalonia/ReactiveUI do aktywacji.")]
    public void Constructor_InitializesWithoutException()
    {
        var view = new LoginView();
        Assert.NotNull(view);
    }
}
