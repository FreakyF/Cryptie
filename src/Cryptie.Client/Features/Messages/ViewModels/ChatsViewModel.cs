using Cryptie.Client.Core.Base;
using ReactiveUI;

namespace Cryptie.Client.Features.Messages.ViewModels;

public class ChatsViewModel(IScreen hostScreen) : RoutableViewModelBase(hostScreen)
{
    public GroupsListViewModel GroupsPanel { get; } = new(hostScreen);
}