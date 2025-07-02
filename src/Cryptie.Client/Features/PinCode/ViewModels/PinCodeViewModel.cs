using System.Reactive;
using Cryptie.Client.Core.Base;
using Cryptie.Client.Core.Navigation;
using ReactiveUI;

namespace Cryptie.Client.Features.PinCode.ViewModels;

public partial class PinCodeViewModel : RoutableViewModelBase
{
    private readonly IShellCoordinator _shellCoordinator;
    private string _pinCode = string.Empty;

    public PinCodeViewModel(IScreen hostScreen, IShellCoordinator shellCoordinator) : base(hostScreen)
    {
        _shellCoordinator = shellCoordinator;
        VerifyCommand = ReactiveCommand.Create(VerifyPin);
    }

    public string PinCode
    {
        get => _pinCode;
        set => this.RaiseAndSetIfChanged(ref _pinCode, value);
    }

    public ReactiveCommand<Unit, Unit> VerifyCommand { get; }

    private void VerifyPin()
    {
        if (string.IsNullOrWhiteSpace(PinCode))
        {
            ErrorMessage = "PIN must be exactly 6 digits.";
            return;
        }

        ErrorMessage = string.Empty;
        _shellCoordinator.ShowDashboard();
    }
}