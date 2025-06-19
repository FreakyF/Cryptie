using System.Collections.ObjectModel;
using Cryptie.Client.Core.Base;
using ReactiveUI;

namespace Cryptie.Client.Features.Groups.ViewModels;

public class GroupsListViewModel(IScreen hostScreen) : RoutableViewModelBase(hostScreen)
{
    public ObservableCollection<string> Friends { get; } =
    [
        "user4.dev", // idx=0  → #F44336 (red)
        "user5.dev", // idx=1  → #E91E63 (pink)
        "user6.dev", // idx=2  → #9C27B0 (purple)
        "user7.dev", // idx=3  → #673AB7 (deep purple)
        "user8.dev", // idx=4  → #3F51B5 (indigo)
        "user9.dev", // idx=5  → #2196F3 (blue)
        "user15.dev", // idx=6  → #03A9F4 (light blue)
        "user16.dev", // idx=7  → #00BCD4 (cyan)
        "user17.dev", // idx=8  → #009688 (teal)
        "user18.dev", // idx=9  → #4CAF50 (green)
        "user19.dev", // idx=10 → #090CAA (dark blue)
        "user70.dev", // idx=11 → #9CB400 (shrek lime)
        "user0.dev", // idx=12 → #09AA56 (bright green)
        "user1.dev", // idx=13 → #FFC107 (amber)
        "user2.dev", // idx=14 → #FF9800 (orange)
        "user3.dev" // idx=15 → #FF5722 (deep orange)
    ];
}