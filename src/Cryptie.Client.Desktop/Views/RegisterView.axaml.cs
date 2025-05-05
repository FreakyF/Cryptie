using Avalonia.ReactiveUI;
using Cryptie.Client.Desktop.ViewModels;
using ReactiveUI;

namespace Cryptie.Client.Desktop.Views;

public partial class RegisterView : ReactiveUserControl<RegisterViewModel>
{
    public RegisterView()
    {
        this.WhenActivated(disposables => { });
        InitializeComponent();
    }
}