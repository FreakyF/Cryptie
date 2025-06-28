using System.Reactive;
using Cryptie.Client.Configuration;
using Cryptie.Client.Core.Base;
using Cryptie.Client.Core.Services;
using Cryptie.Client.Features.AddFriend.Services;
using Cryptie.Client.Features.Authentication.Services;
using Cryptie.Client.Features.Groups.Services;
using Cryptie.Client.Features.Groups.ViewModels;
using Cryptie.Client.Features.Menu.State;
using Cryptie.Common.Features.UserManagement.DTOs;
using FluentValidation;
using Microsoft.Extensions.Options;
using ReactiveUI;

namespace Cryptie.Client.Features.Chats.ViewModels;

/// <summary>
/// Root VM dla ekranu czatu.  Zawiera:
/// • panel listy grup (po lewej)  
/// • flagę otwarcia prawego panelu ustawień  
/// • komendę przełączającą panel ustawień.
/// </summary>
public sealed class ChatsViewModel : RoutableViewModelBase
{
    private bool _isChatSettingsOpen;

    // -------- ctor --------
    public ChatsViewModel(
        IScreen hostScreen,
        IConnectionMonitor connectionMonitor,
        IOptions<ClientOptions> options,
        IFriendsService friendsService,
        IKeychainManagerService keychainManager,
        IValidator<AddFriendRequestDto> validator,
        IGroupService groupService,
        IUserState userState)
        : base(hostScreen)
    {
        GroupsPanel = new GroupsListViewModel(
            hostScreen, connectionMonitor, options,
            friendsService, keychainManager,
            validator, groupService, userState);

        ToggleChatSettingsCommand =
            ReactiveCommand.Create(() => { IsChatSettingsOpen = !IsChatSettingsOpen; });
    }

    public GroupsListViewModel GroupsPanel { get; }

    public ReactiveCommand<Unit, Unit> ToggleChatSettingsCommand { get; }

    public bool IsChatSettingsOpen
    {
        get => _isChatSettingsOpen;
        set => this.RaiseAndSetIfChanged(ref _isChatSettingsOpen, value);
    }
}