using Avalonia.ReactiveUI;
using Cryptie.Client.Desktop.ViewModels;

namespace Cryptie.Client.Desktop.Views;

public partial class LoginView : ReactiveUserControl<LoginViewModel>
{
    public LoginView()
    {
        InitializeComponent();
    }
}