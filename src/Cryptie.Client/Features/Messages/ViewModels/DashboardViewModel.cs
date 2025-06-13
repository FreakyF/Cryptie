using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using Cryptie.Client.Core.Base;
using Cryptie.Client.Features.Messages.Models;
using ReactiveUI;

namespace Cryptie.Client.Features.Messages.ViewModels;

public sealed class DashboardViewModel : RoutableViewModelBase
{
    private bool _isPaneOpen;

    private NavigationItem? _selectedItem;

    public DashboardViewModel(IScreen hostScreen) : base(hostScreen)
    {
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
        new("Friends", "\uE8FA"),
        new("Settings", "\uE713"),
        new("Account", "\uE77B", true)
    ];

    public NavigationItem? SelectedItem
    {
        get => _selectedItem;
        set => this.RaiseAndSetIfChanged(ref _selectedItem, value);
    }

    public ReactiveCommand<Unit, Unit> TogglePaneCommand { get; }
}