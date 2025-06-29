using Avalonia.ReactiveUI;
using Cryptie.Client.Features.AddUserToGroup.ViewModels;

namespace Cryptie.Client.Features.AddUserToGroup.Views;

public partial class AddUserToGroupView : ReactiveUserControl<AddUserToGroupViewModel>
{
    public AddUserToGroupView()
    {
        InitializeComponent();
    }
}