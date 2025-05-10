using Cryptie.Client.Desktop.Core.Base;
using Cryptie.Client.Desktop.Core.Factories;
using Cryptie.Client.Desktop.Features.Authentication.ViewModels;
using ReactiveUI;

namespace Cryptie.Client.Desktop.Core.Navigation;

public class ShellCoordinator(IViewModelFactory factory) : IShellCoordinator
{
    public RoutingState Router { get; } = new();

    public void Start()
    {
        ShowLogin();
    }

    public void ShowLogin()
    {
        NavigateTo<LoginViewModel>();
    }

    public void ShowRegister()
    {
        NavigateTo<RegisterViewModel>();
    }

    private void NavigateTo<TViewModel>()
        where TViewModel : RoutableViewModelBase
    {
        var vm = factory.Create<TViewModel>(this);
        Router.Navigate.Execute(vm);
    }
}