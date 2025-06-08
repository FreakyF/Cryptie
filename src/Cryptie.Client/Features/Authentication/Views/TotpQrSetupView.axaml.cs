using Avalonia.ReactiveUI;
using Cryptie.Client.Desktop.Features.Authentication.ViewModels;

namespace Cryptie.Client.Desktop.Features.Authentication.Views;

public partial class TotpQrSetupView : ReactiveUserControl<TotpQrSetupViewModel>
{
    public TotpQrSetupView()
    {
        InitializeComponent();
    }
}