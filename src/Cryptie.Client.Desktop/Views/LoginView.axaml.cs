using Avalonia.ReactiveUI;
using Cryptie.Client.Desktop.ViewModels;
using ReactiveUI;

namespace Cryptie.Client.Desktop.Views;

public partial class LoginView : ReactiveUserControl<LoginViewModel>
{
    public LoginView()
    {
        this.WhenActivated(disposables => { });
        InitializeComponent();
    }
}