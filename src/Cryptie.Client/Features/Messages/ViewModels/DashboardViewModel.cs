using Cryptie.Client.Core.Base;
using ReactiveUI;

namespace Cryptie.Client.Features.Messages.ViewModels;

public sealed class DashboardViewModel(
    IScreen hostScreen,
    SplitViewMenuViewModel menu) : RoutableViewModelBase(hostScreen)
{
    public SplitViewMenuViewModel Menu { get; } = menu;
}