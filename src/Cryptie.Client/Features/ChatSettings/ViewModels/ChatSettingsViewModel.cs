using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using Cryptie.Client.Configuration;
using Cryptie.Client.Core.Base;
using Cryptie.Client.Features.AddUserToGroup.ViewModels;
using Cryptie.Client.Features.Groups.State;
using Microsoft.Extensions.Options;
using ReactiveUI;

namespace Cryptie.Client.Features.ChatSettings.ViewModels;

public sealed class ChatSettingsViewModel : RoutableViewModelBase
{
    private readonly ObservableAsPropertyHelper<string?> _currentGroupName;
    private readonly ObservableAsPropertyHelper<bool> _isGroupPrivate;

    public ChatSettingsViewModel(
        IScreen hostScreen,
        IGroupSelectionState groupState,
        IOptions<ClientOptions> options,
        AddUserToGroupViewModel addUserToGroupVm)
        : base(hostScreen)
    {
        IconUri = options.Value.FontUri;

        groupState
            .WhenAnyValue(gs => gs.SelectedGroupName)
            .ObserveOn(RxApp.MainThreadScheduler)
            .ToProperty(this, vm => vm.CurrentGroupName, out _currentGroupName);

        groupState
            .WhenAnyValue(gs => gs.IsGroupPrivate)
            .ObserveOn(RxApp.MainThreadScheduler)
            .ToProperty(this, vm => vm.IsGroupPrivate, out _isGroupPrivate);

        AddUsersCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            using var cts = new CancellationTokenSource();
            await ShowAddUserToGroup.Handle((addUserToGroupVm, cts.Token));
        });

        AddUsersCommand.ThrownExceptions
            .Subscribe();
    }

    public string IconUri { get; }

    public ReactiveCommand<Unit, Unit> AddUsersCommand { get; }

    public Interaction<(AddUserToGroupViewModel, CancellationToken), Unit> ShowAddUserToGroup { get; }
        = new();

    public string? CurrentGroupName => _currentGroupName.Value;
    public bool IsGroupPrivate => _isGroupPrivate.Value;
}