using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Cryptie.Client.Core.Base;
using Cryptie.Client.Core.Navigation;
using Cryptie.Client.Features.Authentication.Services;
using Cryptie.Client.Features.Messages.Models;
using Cryptie.Client.Features.Messages.Services;
using Cryptie.Common.Features.UserManagement.DTOs;
using ReactiveUI;

namespace Cryptie.Client.Features.Messages.ViewModels;

/// <summary>
/// View-model lewego panelu nawigacyjnego w Dashboardzie.
/// Zapewnia zarówno rozwijanie/zamykanie panelu, jak i przełączanie widoków
/// poprzez IShellCoordinator.
/// </summary>
public sealed class SplitViewMenuViewModel : ViewModelBase
{
    private readonly IShellCoordinator _coordinator;
    private readonly IKeychainManagerService _keychainManager;
    private readonly IUserDetailsService _userDetails;

    private bool _isPaneOpen;
    private NavigationItem? _selectedItem;

    public SplitViewMenuViewModel(
        IUserDetailsService userDetails,
        IKeychainManagerService keychainManager,
        IShellCoordinator coordinator)
    {
        _userDetails = userDetails;
        _keychainManager = keychainManager;
        _coordinator = coordinator;

        Items =
        [
            new NavigationItem("Chats", "\uE15F", NavigationTarget.Chats),
            new NavigationItem("Account", "\uE168", NavigationTarget.Account, true, true),
            new NavigationItem("Settings", "\uE115", NavigationTarget.Settings, true)
        ];
        _selectedItem = Items.First();

        TogglePaneCommand = ReactiveCommand.Create(() => { IsPaneOpen = !IsPaneOpen; });
        NavigateCommand = ReactiveCommand.Create<NavigationItem>(Navigate);

        // Reagujemy na każdą zmianę SelectedItem
        this.WhenAnyValue(vm => vm.SelectedItem)
            .Where(item => item is not null)
            .InvokeCommand(NavigateCommand);

        _ = LoadUserNameAsync();
    }

    /* ---------- Public API ---------- */

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
    public ReactiveCommand<NavigationItem, Unit> NavigateCommand { get; }

    /* ---------- Implementacja ---------- */

    private void Navigate(NavigationItem item)
    {
        switch (item.Target)
        {
            case NavigationTarget.Chats: _coordinator.ShowChats(); break;
            case NavigationTarget.Account: _coordinator.ShowAccount(); break;
            case NavigationTarget.Settings: _coordinator.ShowSettings(); break;
        }

        // opcjonalnie chowamy panel po kliknięciu
        IsPaneOpen = false;
    }

    private async Task LoadUserNameAsync()
    {
        try
        {
            if (!_keychainManager.TryGetSessionToken(out var tokenStr, out _))
                return;

            if (!Guid.TryParse(tokenStr, out var sessionToken))
                return;

            var guidResponse = await _userDetails.GetUserGuidFromTokenAsync(
                new UserGuidFromTokenRequestDto { SessionToken = sessionToken });

            if (guidResponse is null)
                return;

            var nameResponse = await _userDetails.GetUsernameFromGuidAsync(
                new NameFromGuidRequestDto { Id = guidResponse.Guid });

            var userName = nameResponse?.Name;
            if (string.IsNullOrWhiteSpace(userName))
                return;

            ReplaceAccountItem(userName);
        }
        catch
        {
            // Ignorujemy wyjątki – wyświetli się domyślna etykieta „Account”.
        }
    }

    private void ReplaceAccountItem(string userName)
    {
        var accountItem = Items.FirstOrDefault(i =>
            i is { FullLabel: "Account", IconGlyph: "\uE168" });

        if (accountItem is null)
            return;

        var index = Items.IndexOf(accountItem);
        if (index < 0)
            return;

        Items[index] = accountItem with { FullLabel = userName };
    }
}