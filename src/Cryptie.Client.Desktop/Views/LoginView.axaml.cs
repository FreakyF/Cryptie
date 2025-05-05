using System.Reactive.Disposables;
using Avalonia.ReactiveUI;
using Cryptie.Client.Desktop.ViewModels;
using ReactiveUI;

namespace Cryptie.Client.Desktop.Views;

public partial class LoginView : ReactiveUserControl<LoginViewModel>
{
    public LoginView()
    {
        InitializeComponent();
        this.WhenActivated(disposables =>
        {
            this.Bind(ViewModel,
                    vm => vm.Model.Username,
                    v => v.UsernameTextBox.Text)
                .DisposeWith(disposables);

            this.Bind(ViewModel,
                    vm => vm.Model.Password,
                    v => v.PasswordBox.Text)
                .DisposeWith(disposables);

            this.Bind(ViewModel,
                    vm => vm.ErrorMessage,
                    v => v.ErrorMessageTextBlock.Text)
                .DisposeWith(disposables);

            this.BindCommand(ViewModel,
                    vm => vm.LoginCommand,
                    v => v.LoginButton)
                .DisposeWith(disposables);

            this.BindCommand(ViewModel,
                    vm => vm.GoToRegisterCommand,
                    v => v.SignUpButton)
                .DisposeWith(disposables);
        });
    }
}