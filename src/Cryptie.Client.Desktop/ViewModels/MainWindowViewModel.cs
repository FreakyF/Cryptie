using ReactiveUI;

namespace Cryptie.Client.Desktop.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private readonly IViewModelFactory _viewModelFactory;
    private ViewModelBase _currentViewModel = null!;

    public ViewModelBase CurrentViewModel
    {
        get => _currentViewModel;
        private set => this.RaiseAndSetIfChanged(ref _currentViewModel, value);
    }

    public MainWindowViewModel(IViewModelFactory viewModelFactory)
    {
        _viewModelFactory = viewModelFactory;
        ShowLogin();
    }

    public void ShowLogin() => CurrentViewModel = _viewModelFactory.Create<LoginViewModel>(this);
    public void ShowRegister() => CurrentViewModel = _viewModelFactory.Create<RegisterViewModel>(this);
}