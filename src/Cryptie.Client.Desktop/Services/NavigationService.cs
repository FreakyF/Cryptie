using Cryptie.Client.Desktop.ViewModels;

namespace Cryptie.Client.Desktop.Services;

public class NavigationService(MainWindowViewModel shell) : INavigationService
{
    public void NavigateToLogin() => shell.ShowLogin();
    public void NavigateToRegister() => shell.ShowRegister();
}