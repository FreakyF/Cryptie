using System.Collections.ObjectModel;
using Cryptie.Client.Core.Base;
using ReactiveUI;

namespace Cryptie.Client.Features.Groups.ViewModels;

public class GroupsListViewModel(IScreen hostScreen) : RoutableViewModelBase(hostScreen)
{
    public ObservableCollection<string> Friends { get; } =
    [
        "charlie.dev",
        "charlie.dev",
        "charlie.dev",
        "charlie.dev",
        "charlie.dev",
        "charlie.dev",
        "charlie.dev",
        "charlie.dev",
        "charlie.dev",
        "charlie.dev",
        "charlie.dev",
        "charlie.dev",
        "charlie.dev",
        "charlie.dev",
        "charlie.dev",
        "charlie.dev",
        "charlie.dev",
        "charlie.dev",
        "charlie.dev",
    ];
}