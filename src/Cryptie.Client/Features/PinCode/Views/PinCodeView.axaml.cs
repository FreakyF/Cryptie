using Avalonia.ReactiveUI;
using Cryptie.Client.Core.Filters;
using Cryptie.Client.Features.PinCode.ViewModels;

namespace Cryptie.Client.Features.PinCode.Views;

// ReSharper disable once UnusedType.Global
public partial class PinCodeView : ReactiveUserControl<PinCodeViewModel>
{
    public PinCodeView()
    {
        InitializeComponent();
        DigitOnlyInputFilter.Attach(PinCodeTextBox);
    }
}