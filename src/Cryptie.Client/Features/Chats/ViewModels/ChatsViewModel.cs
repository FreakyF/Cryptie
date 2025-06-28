using System.Reactive;
using System.Reactive.Linq;
using Cryptie.Client.Configuration;
using Cryptie.Client.Core.Base;
using Cryptie.Client.Core.Services;
using Cryptie.Client.Features.ChatSettings.ViewModels;
using Cryptie.Client.Features.Groups.Dependencies;
using Cryptie.Client.Features.Groups.Services;
using Cryptie.Client.Features.Groups.State;
using Cryptie.Client.Features.Groups.ViewModels;
using Microsoft.Extensions.Options;
using ReactiveUI;

namespace Cryptie.Client.Features.Chats.ViewModels;

public sealed class ChatsViewModel : RoutableViewModelBase
{
    private readonly ObservableAsPropertyHelper<string?> _currentGroupName;
    private bool _isChatSettingsOpen;

    public ChatsViewModel(
        IScreen hostScreen,
        IConnectionMonitor connectionMonitor,
        IOptions<ClientOptions> options,
        AddFriendDependencies deps,
        IGroupService groupService,
        IGroupSelectionState groupState,
        ChatSettingsViewModel settingsPanel)
        : base(hostScreen)
    {
        GroupsPanel = new GroupsListViewModel(
            hostScreen,
            connectionMonitor,
            options,
            deps,
            groupService,
            groupState);

        SettingsPanel = settingsPanel;

        groupState
            .WhenAnyValue(gs => gs.SelectedGroupName)
            .ObserveOn(RxApp.MainThreadScheduler)
            .ToProperty(this, vm => vm.CurrentGroupName, out _currentGroupName);

        ToggleChatSettingsCommand =
            ReactiveCommand.Create(() => { IsChatSettingsOpen = !IsChatSettingsOpen; });
    }

    public GroupsListViewModel GroupsPanel { get; }

    public ChatSettingsViewModel SettingsPanel { get; }

    public string? CurrentGroupName => _currentGroupName.Value;

    public ReactiveCommand<Unit, Unit> ToggleChatSettingsCommand { get; }

    public bool IsChatSettingsOpen
    {
        get => _isChatSettingsOpen;
        set => this.RaiseAndSetIfChanged(ref _isChatSettingsOpen, value);
    }
}