using Avalonia.ReactiveUI;
using Cryptie.Client.Desktop.ViewModels;

namespace Cryptie.Client.Desktop.Views;

public partial class RegisterView : ReactiveUserControl<RegisterViewModel>
{
    public RegisterView()
    {
        InitializeComponent();
    }
}