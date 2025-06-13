using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using Cryptie.Client.Core.Base;
using Cryptie.Client.Features.Messages.Models;
using Cryptie.Client.Features.Messages.Services;
using ReactiveUI;

namespace Cryptie.Client.Features.Messages.ViewModels;

public sealed class DashboardViewModel : RoutableViewModelBase
{
    private readonly IUserDetailsService _userDetailsService;
    private bool _isPaneOpen;

    private NavigationItem? _selectedItem;

    public DashboardViewModel(IScreen hostScreen, IUserDetailsService userDetailsService) : base(hostScreen)
    {
        _userDetailsService = userDetailsService;
        _selectedItem = Items.FirstOrDefault();

        TogglePaneCommand = ReactiveCommand.Create(() => { IsPaneOpen = !IsPaneOpen; });
    }

    public bool IsPaneOpen
    {
        get => _isPaneOpen;
        set => this.RaiseAndSetIfChanged(ref _isPaneOpen, value);
    }

    public ObservableCollection<NavigationItem> Items { get; } =
    [
        new("Chats", "\uE15F"),
        new("Account", "\uE168", true, true),
        new("Settings", "\uE115", true)
    ];

    public NavigationItem? SelectedItem
    {
        get => _selectedItem;
        set => this.RaiseAndSetIfChanged(ref _selectedItem, value);
    }

    public ReactiveCommand<Unit, Unit> TogglePaneCommand { get; }
}