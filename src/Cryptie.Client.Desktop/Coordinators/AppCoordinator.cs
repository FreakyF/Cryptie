using Cryptie.Client.Desktop.Composition.Factories;
using Cryptie.Client.Desktop.ViewModels;
using ReactiveUI;

namespace Cryptie.Client.Desktop.Coordinators;

public class AppCoordinator(IViewModelFactory factory) : IAppCoordinator
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