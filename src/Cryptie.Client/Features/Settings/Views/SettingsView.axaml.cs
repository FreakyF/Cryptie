using Avalonia.ReactiveUI;
using Cryptie.Client.Features.Settings.ViewModels;

namespace Cryptie.Client.Features.Settings.Views;

public partial class SettingsView : ReactiveUserControl<SettingsViewModel>
{
    public SettingsView()
    {
        InitializeComponent();
    }
}