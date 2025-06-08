using Avalonia.ReactiveUI;
using Cryptie.Client.Desktop.Features.Authentication.ViewModels;

namespace Cryptie.Client.Desktop.Features.Authentication.Views;

public partial class TotpCodeView : ReactiveUserControl<TotpCodeViewModel>
{
    public TotpCodeView()
    {
        InitializeComponent();
    }
}