using Cryptie.Client.Core.Base;
using Cryptie.Client.Features.Groups.ViewModels;
using ReactiveUI;

namespace Cryptie.Client.Features.Chats.ViewModels;

public class ChatsViewModel(IScreen hostScreen) : RoutableViewModelBase(hostScreen)
{
    public GroupsListViewModel GroupsPanel { get; } = new(hostScreen);
}