using Cryptie.Client.Core.Base;
using Cryptie.Client.Core.Services;
using Cryptie.Client.Features.Groups.ViewModels;
using ReactiveUI;

namespace Cryptie.Client.Features.Chats.ViewModels;

public class ChatsViewModel(IScreen hostScreen, IConnectionMonitor connectionMonitor)
    : RoutableViewModelBase(hostScreen)
{
    public GroupsListViewModel GroupsPanel { get; } = new(hostScreen, connectionMonitor);
}