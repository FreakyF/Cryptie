using Avalonia.ReactiveUI;
using Cryptie.Client.Features.Chats.ViewModels;

namespace Cryptie.Client.Features.Chats.Views;

public partial class ChatsView : ReactiveUserControl<ChatsViewModel>
{
    public ChatsView()
    {
        InitializeComponent();
    }
}