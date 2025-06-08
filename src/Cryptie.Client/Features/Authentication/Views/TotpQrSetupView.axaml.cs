using Avalonia.ReactiveUI;
using Cryptie.Client.Features.Authentication.ViewModels;

namespace Cryptie.Client.Features.Authentication.Views;

public partial class TotpQrSetupView : ReactiveUserControl<TotpQrSetupViewModel>
{
    public TotpQrSetupView()
    {
        InitializeComponent();
    }
}