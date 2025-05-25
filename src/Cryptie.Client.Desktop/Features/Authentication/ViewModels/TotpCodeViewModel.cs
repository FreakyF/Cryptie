using Cryptie.Client.Desktop.Core.Base;
using ReactiveUI;

namespace Cryptie.Client.Desktop.Features.Authentication.ViewModels;

public class TotpCodeViewModel : RoutableViewModelBase
{
    public TotpCodeViewModel(IScreen hostScreen) : base(hostScreen)
    {
    }
}