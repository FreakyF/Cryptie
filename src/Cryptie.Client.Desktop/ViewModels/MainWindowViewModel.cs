using ReactiveUI;

namespace Cryptie.Client.Desktop.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private ViewModelBase _currentViewModel;
    private readonly IAuthApiService _authApi;

    public ViewModelBase CurrentViewModel
    {
        get => _currentViewModel;
        set => this.RaiseAndSetIfChanged(ref _currentViewModel, value);
    }

    public MainWindowViewModel(IAuthApiService authApi)
    {
        _authApi = authApi;
        _currentViewModel = new LoginViewModel(this);
    }

    public void ShowRegister() => CurrentViewModel = new RegisterViewModel(_authApi, this);

    public void ShowLogin() => CurrentViewModel = new LoginViewModel(this);
}