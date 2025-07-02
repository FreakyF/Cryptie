using Avalonia.ReactiveUI;
using Cryptie.Client.Core.Filters;
using Cryptie.Client.Features.Authentication.ViewModels;

namespace Cryptie.Client.Features.Authentication.Views;

// ReSharper disable once UnusedType.Global
public partial class TotpCodeView : ReactiveUserControl<TotpCodeViewModel>
{
    public TotpCodeView()
    {
        InitializeComponent();
        DigitOnlyInputFilter.Attach(AuthCodeTextBox);
    }
}