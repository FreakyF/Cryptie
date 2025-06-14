using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Avalonia.ReactiveUI;
using Cryptie.Client.Core.Base;
using Cryptie.Client.Core.Services;
using Cryptie.Client.Features.Authentication.Services;
using Cryptie.Client.Features.Messages.Models;
using Cryptie.Client.Features.Messages.Services;
using Cryptie.Common.Features.UserManagement.DTOs;
using ReactiveUI;

namespace Cryptie.Client.Features.Messages.ViewModels
{
    public sealed class SplitViewMenuViewModel : ViewModelBase, IDisposable
    {
        private readonly IDisposable _connSub;
        private readonly IKeychainManagerService _keychainManager;
        private readonly IUserDetailsService _userDetails;
        private bool _disposed;
        private bool _isPaneOpen;
        private NavigationItem? _selectedItem;

        public SplitViewMenuViewModel(
            IUserDetailsService userDetails,
            IKeychainManagerService keychainManager,
            IConnectionMonitor connectionMonitor)
        {
            _userDetails = userDetails;
            _keychainManager = keychainManager;

            Items =
            [
                new NavigationItem("Chats", "\uE15F", NavigationTarget.Chats),
                new NavigationItem("Account", "\uE168", NavigationTarget.Account, IsBottom: true, IsLast: true),
                new NavigationItem("Settings", "\uE115", NavigationTarget.Settings, IsBottom: true)
            ];
            _selectedItem = Items[0];

            TogglePaneCommand = ReactiveCommand.Create(() => { IsPaneOpen = !IsPaneOpen; });
            NavigateCommand = ReactiveCommand.Create<NavigationItem>(Navigate);

            this.WhenAnyValue(vm => vm.SelectedItem)
                .Where(x => x is not null)
                .Select(x => x!)
                .InvokeCommand(NavigateCommand);

            _connSub = connectionMonitor.ConnectionStatusChanged
                .StartWith(false)
                .Where(isUp => isUp)
                .ObserveOn(AvaloniaScheduler.Instance)
                .Subscribe(isUp => { _ = LoadUserNameAsync(); });
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
        private ReactiveCommand<NavigationItem, Unit> NavigateCommand { get; }

        public void Dispose()
        {
            if (_disposed) return;
            _connSub.Dispose();
            _disposed = true;
        }

        public event Action<NavigationTarget>? MenuItemSelected;
        private void Navigate(NavigationItem item) => MenuItemSelected?.Invoke(item.Target);

        private async Task LoadUserNameAsync()
        {
            try
            {
                if (!_keychainManager.TryGetSessionToken(out var tokenStr, out _))
                    return;
                if (!Guid.TryParse(tokenStr, out var sessionToken))
                    return;

                var guidResp = await _userDetails.GetUserGuidFromTokenAsync(
                    new UserGuidFromTokenRequestDto { SessionToken = sessionToken });
                if (guidResp is null) return;

                var nameResp = await _userDetails.GetUsernameFromGuidAsync(
                    new NameFromGuidRequestDto { Id = guidResp.Guid });
                var userName = nameResp?.Name;
                if (string.IsNullOrWhiteSpace(userName)) return;

                ReplaceAccountItem(userName);
            }
            catch
            {
                // Swallowing any exceptions so execution continues and the default “Account” view is rendered.
            }
        }

        private void ReplaceAccountItem(string userName)
        {
            var accountItem = Items.FirstOrDefault(i =>
                i is { FullLabel: "Account", IconGlyph: "\uE168" });
            if (accountItem is null) return;
            var idx = Items.IndexOf(accountItem);
            if (idx < 0) return;

            Items[idx] = accountItem with { FullLabel = userName };
        }
    }
}