using System.Reactive.Disposables;
using Avalonia.ReactiveUI;
using Cryptie.Client.Desktop.ViewModels;
using ReactiveUI;

namespace Cryptie.Client.Desktop.Views;

public partial class RegisterView : ReactiveUserControl<RegisterViewModel>
{
    public RegisterView()
    {
        InitializeComponent();
        this.WhenActivated(disposables =>
        {
            this.Bind(ViewModel,
                    vm => vm.Model.Username,
                    v  => v.UsernameTextBox.Text)
                .DisposeWith(disposables);

            this.Bind(ViewModel,
                    vm => vm.Model.DisplayName,
                    v  => v.DisplayNameTextBox.Text)
                .DisposeWith(disposables);

            this.Bind(ViewModel,
                    vm => vm.Model.Email,
                    v  => v.EmailTextBox.Text)
                .DisposeWith(disposables);

            this.Bind(ViewModel,
                    vm => vm.Model.Password,
                    v  => v.PasswordTextBox.Text)
                .DisposeWith(disposables);

            this.Bind(ViewModel,
                    vm => vm.errorMessage,
                    v  => v.ErrorMessageTextBlock.Text)
                .DisposeWith(disposables);

            this.BindCommand(ViewModel,
                    vm => vm.RegisterCommand,
                    v  => v.RegisterButton)
                .DisposeWith(disposables);

            this.BindCommand(ViewModel,
                    vm => vm.GoToLoginCommand,
                    v  => v.LoginButton)
                .DisposeWith(disposables);
        });
    }
}