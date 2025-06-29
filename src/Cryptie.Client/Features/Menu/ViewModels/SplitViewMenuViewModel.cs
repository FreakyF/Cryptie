using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Avalonia.ReactiveUI;
using Cryptie.Client.Core.Base;
using Cryptie.Client.Core.Navigation;
using Cryptie.Client.Core.Services;
using Cryptie.Client.Features.Menu.Models;
using Cryptie.Client.Features.Menu.State;
using Cryptie.Common.Features.UserManagement.DTOs;
using ReactiveUI;

namespace Cryptie.Client.Features.Menu.ViewModels;

public sealed class SplitViewMenuViewModel : ViewModelBase, IDisposable
{
    private readonly IDisposable _connSub;
    private readonly IUserDetailsService _userDetails;
    private readonly IDisposable _userNameSub;
    private readonly IUserState _userState;
    private bool _disposed;
    private bool _isPaneOpen;
    private NavigationItem? _selectedItem;

    public SplitViewMenuViewModel(
        IUserDetailsService userDetails,
        IConnectionMonitor connectionMonitor,
        IUserState userState)
    {
        _userState = userState;
        _userDetails = userDetails;

        Items =
        [
            new NavigationItem("Chats", "\uE168", c => c.ShowChats()),
            new NavigationItem("Account", "\uE4C2", c => c.ShowAccount(), IsBottom: true, IsLast: true),
            new NavigationItem("Settings", "\uE272", c => c.ShowSettings(), IsBottom: true),
        ];
        _selectedItem = Items[0];

        TogglePaneCommand = ReactiveCommand.Create(() => { IsPaneOpen = !IsPaneOpen; });
        NavigateCommand = ReactiveCommand.Create<NavigationItem>(Navigate);

        this.WhenAnyValue(vm => vm.SelectedItem)
            .Where(x => x is not null)
            .Select(x => x!)
            .InvokeCommand(NavigateCommand);

        _connSub = connectionMonitor.ConnectionStatusChanged
            .StartWith(true)
            .Where(isUp => isUp)
            .ObserveOn(AvaloniaScheduler.Instance)
            .Subscribe(isUp => { _ = LoadUserNameAsync(); });

        _userNameSub = _userState
            .WhenAnyValue(s => s.Username)
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .ObserveOn(AvaloniaScheduler.Instance)
            .Subscribe(name => ReplaceAccountItem(name!));
    }

    public IContentCoordinator? ContentCoordinator { get; set; }

    public bool IsPaneOpen
    {
        get => _isPaneOpen;
        set => this.RaiseAndSetIfChanged(ref _isPaneOpen, value);
    }

    public ObservableCollection<NavigationItem> Items { get; }

    public NavigationItem? SelectedItem
    {
        get => _selectedItem;
        set => this.RaiseAndSetIfChanged(ref _selectedItem, value);
    }

    public ReactiveCommand<Unit, Unit> TogglePaneCommand { get; }
    private ReactiveCommand<NavigationItem, Unit> NavigateCommand { get; }

    public void Dispose()
    {
        if (_disposed) return;
        _connSub.Dispose();
        _userNameSub.Dispose();
        _disposed = true;
    }

    private void Navigate(NavigationItem item)
    {
        if (ContentCoordinator != null)
        {
            item.NavigateAction(ContentCoordinator);
        }
    }

    private async Task LoadUserNameAsync()
    {
        try
        {
            var tokenString = _userState.SessionToken;
            if (string.IsNullOrWhiteSpace(tokenString) ||
                !Guid.TryParse(tokenString, out var sessionToken))
            {
                return;
            }

            var guidResp = await _userDetails.GetUserGuidFromTokenAsync(
                new UserGuidFromTokenRequestDto { SessionToken = sessionToken });
            if (guidResp is null) return;

            var nameResp = await _userDetails.GetUsernameFromGuidAsync(
                new NameFromGuidRequestDto { Id = guidResp.Guid });
            var userName = nameResp?.Name;
            if (string.IsNullOrWhiteSpace(userName)) return;
            _userState.Username = userName;
            ReplaceAccountItem(userName);
        }
        catch
        {
            // Swallowing any exceptions so execution continues and the default “Account” view is rendered.
        }
    }

    private void ReplaceAccountItem(string userName)
    {
        var oldItem = Items.FirstOrDefault(i => i.IconGlyph == "\uE4C2");
        if (oldItem is null) return;

        var idx = Items.IndexOf(oldItem);
        if (idx < 0) return;

        Items[idx] = oldItem with { FullLabel = userName };
    }
}