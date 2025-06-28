using Avalonia.ReactiveUI;
using Cryptie.Client.Features.ChatSettings.ViewModels;

namespace Cryptie.Client.Features.ChatSettings.Views;

public partial class ChatSettingsView : ReactiveUserControl<ChatSettingsViewModel>
{
    public ChatSettingsView()
    {
        InitializeComponent();
    }
}