using Cryptie.Client.Desktop.Composition.Factories;
using ReactiveUI;

namespace Cryptie.Client.Desktop.ViewModels;

public class MainWindowViewModel : ReactiveObject, IScreen
{
    private readonly IViewModelFactory _viewModelFactory;

    public MainWindowViewModel(IViewModelFactory viewModelFactory)
    {
        _viewModelFactory = viewModelFactory;
        ShowLogin();
    }

    public RoutingState Router { get; } = new();

    public void ShowLogin()
    {
        Router.Navigate.Execute(
            _viewModelFactory.Create<LoginViewModel>(this));
    }

    public void ShowRegister()
    {
        Router.Navigate.Execute(
            _viewModelFactory.Create<RegisterViewModel>(this));
    }
}