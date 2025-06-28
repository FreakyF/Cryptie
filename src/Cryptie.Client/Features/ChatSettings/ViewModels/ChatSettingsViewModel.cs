using System;
using System.Reactive.Linq;
using Cryptie.Client.Core.Base;
using Cryptie.Client.Features.Groups.State;
using ReactiveUI;

namespace Cryptie.Client.Features.ChatSettings.ViewModels;

public sealed class ChatSettingsViewModel : RoutableViewModelBase
{
    private readonly ObservableAsPropertyHelper<string?> _currentGroupName;
    private readonly ObservableAsPropertyHelper<bool> _isGroupPrivate;

    public ChatSettingsViewModel(
        IScreen hostScreen,
        IGroupSelectionState groupState)
        : base(hostScreen)
    {
        groupState
            .WhenAnyValue(gs => gs.SelectedGroupName)
            .ObserveOn(RxApp.MainThreadScheduler)
            .ToProperty(this, vm => vm.CurrentGroupName, out _currentGroupName);

        groupState
            .WhenAnyValue(gs => gs.IsGroupPrivate)
            .Do(isPriv => { Console.WriteLine($"[Debug] IsGroupPrivate = {isPriv}"); })
            .ObserveOn(RxApp.MainThreadScheduler)
            .ToProperty(this, vm => vm.IsGroupPrivate, out _isGroupPrivate);
    }

    public string? CurrentGroupName => _currentGroupName.Value;
    public bool IsGroupPrivate => _isGroupPrivate.Value;
}