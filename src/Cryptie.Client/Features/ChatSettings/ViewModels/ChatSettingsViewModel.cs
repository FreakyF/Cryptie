using System.Reactive.Linq;
using Cryptie.Client.Core.Base;
using Cryptie.Client.Features.Groups.State;
using ReactiveUI;

namespace Cryptie.Client.Features.ChatSettings.ViewModels;

public sealed class ChatSettingsViewModel : RoutableViewModelBase
{
    private readonly ObservableAsPropertyHelper<string?> _currentGroupName;

    public ChatSettingsViewModel(
        IScreen hostScreen,
        IGroupSelectionState groupState)
        : base(hostScreen)
    {
        groupState
            .WhenAnyValue(gs => gs.SelectedGroupName)
            .ObserveOn(RxApp.MainThreadScheduler)
            .ToProperty(this, vm => vm.CurrentGroupName, out _currentGroupName);
    }

    public string? CurrentGroupName => _currentGroupName.Value;
}