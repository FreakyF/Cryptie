using Avalonia.ReactiveUI;
using Cryptie.Client.Features.Messages.ViewModels;

namespace Cryptie.Client.Features.Messages.Views;

public partial class SettingsView : ReactiveUserControl<SettingsViewModel>
{
    public SettingsView()
    {
        InitializeComponent();
    }
}