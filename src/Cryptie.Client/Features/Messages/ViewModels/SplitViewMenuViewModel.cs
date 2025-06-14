using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using Cryptie.Client.Core.Base;
using Cryptie.Client.Features.Authentication.Services;
using Cryptie.Client.Features.Messages.Models;
using Cryptie.Client.Features.Messages.Services;
using Cryptie.Common.Features.UserManagement.DTOs;
using ReactiveUI;

namespace Cryptie.Client.Features.Messages.ViewModels;

public class SplitViewMenuViewModel : ViewModelBase
{
    private readonly IKeychainManagerService _keychainManager;
    private readonly IUserDetailsService _userDetails;
    private bool _isPaneOpen;

    private NavigationItem? _selectedItem;

    public SplitViewMenuViewModel(
        IUserDetailsService userDetails,
        IKeychainManagerService keychainManager
    )
    {
        _userDetails = userDetails;
        _keychainManager = keychainManager;

        Items =
        [
            new NavigationItem("Chats", "\uE15F"),
            new NavigationItem("Account", "\uE168", true, true),
            new NavigationItem("Settings", "\uE115", true)
        ];
        _selectedItem = Items.FirstOrDefault();

        TogglePaneCommand = ReactiveCommand.Create(() => { IsPaneOpen = !IsPaneOpen; });

        _ = LoadUserNameAsync();
    }

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

    private async Task LoadUserNameAsync()
    {
        try
        {
            if (!_keychainManager.TryGetSessionToken(out var tokenStr, out _))
            {
                return;
            }

            if (!Guid.TryParse(tokenStr, out var sessionToken))
            {
                return;
            }

            var guidResponse = await _userDetails.GetUserGuidFromTokenAsync(new UserGuidFromTokenRequestDto
                { SessionToken = sessionToken });

            if (guidResponse is null)
            {
                return;
            }

            var nameResponse =
                await _userDetails.GetUsernameFromGuidAsync(new NameFromGuidRequestDto { Id = guidResponse.Guid });

            var userName = nameResponse?.Name;
            if (string.IsNullOrWhiteSpace(userName))
            {
                return;
            }

            ReplaceAccountItem(userName);
        }
        catch
        {
            // Swallowing any exceptions so execution continues and the default “Account” view is rendered.
        }
    }

    private void ReplaceAccountItem(string userName)
    {
        var accountItem = Items.FirstOrDefault(i => i is { FullLabel: "Account", IconGlyph: "\uE168" });
        if (accountItem is null) return;

        var index = Items.IndexOf(accountItem);
        if (index < 0) return;

        Items[index] = accountItem with { FullLabel = userName };
    }
}