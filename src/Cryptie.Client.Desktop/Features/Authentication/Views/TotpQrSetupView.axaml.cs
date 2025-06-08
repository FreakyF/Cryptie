using Avalonia.ReactiveUI;
using Cryptie.Client.Desktop.Features.Authentication.ViewModels;



public partial class TotpQrSetupView : ReactiveUserControl<TotpQrSetupViewModel>
{
    public TotpQrSetupView()
    {
        InitializeComponent();
    }
}