using System;
using System.Reactive;
using System.Reactive.Disposables;
using Avalonia.Controls;
using Avalonia.Platform;
using Avalonia.ReactiveUI;
using Cryptie.Client.Features.AddUserToGroup.Views;
using Cryptie.Client.Features.ChatSettings.ViewModels;
using ReactiveUI;

namespace Cryptie.Client.Features.ChatSettings.Views;

public partial class ChatSettingsView : ReactiveUserControl<ChatSettingsViewModel>
{
    public ChatSettingsView()
    {
        InitializeComponent();

        this.WhenActivated(disposables =>
        {
            ViewModel!
                .ShowAddUserToGroup
                .RegisterHandler(async interaction =>
                {
                    var (addUserVm, cancellationToken) = interaction.Input;

                    var dlg = new Window
                    {
                        Title = "Add users to group",
                        Icon = new WindowIcon(
                            AssetLoader.Open(new Uri(ViewModel.IconUri))
                        ),
                        Content = new AddUserToGroupView
                        {
                            DataContext = addUserVm
                        },
                        Width = 400,
                        Height = 150,
                        WindowStartupLocation = WindowStartupLocation.CenterOwner
                    };

                    await using (cancellationToken.Register(dlg.Close))
                    {
                        await dlg.ShowDialog((Window)VisualRoot!);
                    }

                    interaction.SetOutput(Unit.Default);
                })
                .DisposeWith(disposables);
        });
    }
}