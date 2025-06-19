using Avalonia.ReactiveUI;
using Cryptie.Client.Features.Groups.ViewModels;

namespace Cryptie.Client.Features.Groups.Views;

public partial class GroupsListView : ReactiveUserControl<GroupsListViewModel>
{
    public GroupsListView()
    {
        InitializeComponent();
    }
}