﻿using System;
using System.Reactive;
using System.Reactive.Disposables;
using Avalonia.Controls;
using Avalonia.Platform;
using Avalonia.ReactiveUI;
using Cryptie.Client.Features.AddFriend.Views;
using Cryptie.Client.Features.Groups.ViewModels;
using ReactiveUI;

namespace Cryptie.Client.Features.Groups.Views;

public partial class GroupsListView : ReactiveUserControl<GroupsListViewModel>
{
    public GroupsListView()
    {
        InitializeComponent();

        this.WhenActivated(disposables =>
        {
            ViewModel!.ShowAddFriend.RegisterHandler(async interaction =>
            {
                var (addFriendVm, cancellationToken) = interaction.Input;

                var window = new Window
                {
                    Title = "Add Friend",
                    Icon = new WindowIcon(AssetLoader.Open(new Uri(ViewModel.IconUri))),
                    Content = new AddFriendView
                    {
                        DataContext = addFriendVm
                    },
                    Width = 400,
                    Height = 150,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner
                };

                await using (cancellationToken.Register(window.Close))
                {
                    await window.ShowDialog((Window)VisualRoot!);
                }

                interaction.SetOutput(Unit.Default);
            }).DisposeWith(disposables);
        });
    }
}