using Avalonia.ReactiveUI;
using Cryptie.Client.Core.Filters;
using Cryptie.Client.Features.PinCode.ViewModels;

namespace Cryptie.Client.Features.PinCode.Views;

public partial class PinCodeView : ReactiveUserControl<PinCodeViewModel>
{
    public PinCodeView()
    {
        InitializeComponent();
        DigitOnlyInputFilter.Attach(PinCodeTextBox);
    }
}