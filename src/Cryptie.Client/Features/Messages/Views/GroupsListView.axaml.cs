using Avalonia.ReactiveUI;
using Cryptie.Client.Features.Messages.ViewModels;

namespace Cryptie.Client.Features.Messages.Views;

public partial class GroupsListView : ReactiveUserControl<GroupsListViewModel>
{
    public GroupsListView()
    {
        InitializeComponent();
    }
}