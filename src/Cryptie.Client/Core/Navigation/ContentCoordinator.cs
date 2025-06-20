﻿using Cryptie.Client.Core.Factories;
using Cryptie.Client.Features.Account.ViewModels;
using Cryptie.Client.Features.Chats.ViewModels;
using Cryptie.Client.Features.Dashboard.ViewModels;
using Cryptie.Client.Features.Settings.ViewModels;
using ReactiveUI;

namespace Cryptie.Client.Core.Navigation;

public class ContentCoordinator(DashboardViewModel dashboard, IViewModelFactory factory, IScreen screen)
    : IContentCoordinator
{
    public void ShowChats()
    {
        dashboard.Content = factory.Create<ChatsViewModel>(screen);
    }

    public void ShowAccount()
    {
        dashboard.Content = factory.Create<AccountViewModel>(screen);
    }

    public void ShowSettings()
    {
        dashboard.Content = factory.Create<SettingsViewModel>(screen);
    }
}