using Avalonia.ReactiveUI;
using Cryptie.Client.Desktop.Features.Authentication.ViewModels;



public partial class TotpCodeView : ReactiveUserControl<TotpCodeViewModel>
{
    public TotpCodeView()
    {
        InitializeComponent();
    }
}