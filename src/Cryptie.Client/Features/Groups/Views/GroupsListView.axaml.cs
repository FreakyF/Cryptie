using System.Reactive;
using System.Reactive.Disposables;
using Avalonia.Controls;
using Avalonia.ReactiveUI;
using Cryptie.Client.Features.AddFriend.Views;
using Cryptie.Client.Features.Groups.ViewModels;
using ReactiveUI;

// +

namespace Cryptie.Client.Features.Groups.Views;

public partial class GroupsListView : ReactiveUserControl<GroupsListViewModel>
{
    public GroupsListView()
    {
        InitializeComponent();

        this.WhenActivated(disposables =>
        {
            // The handler now receives the context with the ViewModel and CancellationToken
            ViewModel!.ShowAddFriend.RegisterHandler(async interaction => // +/-
            {
                // Deconstruct the tuple from the interaction's input
                var (addFriendVm, cancellationToken) = interaction.Input; // +

                var window = new Window
                {
                    Content = new AddFriendView
                    {
                        // Assign the correct ViewModel from the tuple
                        DataContext = addFriendVm // +/-
                    },
                    Width = 400,
                    Height = 200,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner
                };

                // When the token is cancelled, it will automatically call window.Close()
                using (cancellationToken.Register(window.Close)) // +
                {
                    await window.ShowDialog((Window)VisualRoot!);
                } // + The 'using' block ensures the registration is disposed

                interaction.SetOutput(Unit.Default);
            }).DisposeWith(disposables);
        });
    }
}