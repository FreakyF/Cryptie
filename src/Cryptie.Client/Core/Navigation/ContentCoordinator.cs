using Cryptie.Client.Core.Factories;
using Cryptie.Client.Features.Account.ViewModels;
using Cryptie.Client.Features.Chats.ViewModels;
using Cryptie.Client.Features.Dashboard.ViewModels;
using Cryptie.Client.Features.Settings.ViewModels;
using ReactiveUI;

namespace Cryptie.Client.Core.Navigation;

public class ContentCoordinator(DashboardViewModel dashboard, IViewModelFactory factory, IScreen screen)
    : IContentCoordinator
{
    /// <summary>
    ///     Displays the chat list within the dashboard content region.
    /// </summary>
    public void ShowChats()
    {
        dashboard.Content = factory.Create<ChatsViewModel>(screen);
    }

    /// <summary>
    ///     Displays the current user's account view.
    /// </summary>
    public void ShowAccount()
    {
        dashboard.Content = factory.Create<AccountViewModel>(screen);
    }

    /// <summary>
    ///     Displays the application settings view.
    /// </summary>
    public void ShowSettings()
    {
        dashboard.Content = factory.Create<SettingsViewModel>(screen);
    }
}