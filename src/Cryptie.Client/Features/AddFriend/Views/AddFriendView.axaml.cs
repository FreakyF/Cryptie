using Avalonia.ReactiveUI;
using Cryptie.Client.Features.AddFriend.ViewModels;

namespace Cryptie.Client.Features.AddFriend.Views;

public partial class AddFriendView : ReactiveUserControl<AddFriendViewModel>
{
    public AddFriendView()
    {
        InitializeComponent();
    }
}