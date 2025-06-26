using Cryptie.Client.Configuration;
using Cryptie.Client.Core.Base;
using Cryptie.Client.Core.Services;
using Cryptie.Client.Features.AddFriend.Services;
using Cryptie.Client.Features.Authentication.Services;
using Cryptie.Client.Features.Groups.ViewModels;
using Microsoft.Extensions.Options;
using ReactiveUI;

namespace Cryptie.Client.Features.Chats.ViewModels;

public class ChatsViewModel(
    IScreen hostScreen,
    IConnectionMonitor connectionMonitor,
    IOptions<ClientOptions> options,
    IFriendsService friendsService,
    IKeychainManagerService keychainManager)
    : RoutableViewModelBase(hostScreen)
{
    public GroupsListViewModel GroupsPanel { get; } =
        new(hostScreen, connectionMonitor, options, friendsService, keychainManager);
}