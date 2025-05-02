using ReactiveUI;

namespace Cryptie.Client.Desktop.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private ViewModelBase _currentViewModel;

    public ViewModelBase CurrentViewModel
    {
        get => _currentViewModel;
        set => this.RaiseAndSetIfChanged(ref _currentViewModel, value);
    }

    public MainWindowViewModel()
    {
        _currentViewModel = new LoginViewModel(this);
    }

    public void ShowRegister() => CurrentViewModel = new RegisterViewModel(this);

    public void ShowLogin() => CurrentViewModel = new LoginViewModel(this);
}