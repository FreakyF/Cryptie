using Cryptie.Client.Core.Base;
using Cryptie.Client.Core.Factories;
using Cryptie.Client.Features.Messages.Models;
using ReactiveUI;

namespace Cryptie.Client.Features.Messages.ViewModels;

public sealed class DashboardViewModel : RoutableViewModelBase
{
    private ViewModelBase _content = null!;

    public DashboardViewModel(
        IScreen hostScreen,
        SplitViewMenuViewModel menu,
        IViewModelFactory vmFactory)
        : base(hostScreen)
    {
        var factory = vmFactory;
        Menu = menu;

        Content = factory.Create<ChatsViewModel>(HostScreen);

        Menu.MenuItemSelected += target =>
        {
            Content = target switch
            {
                NavigationTarget.Chats => factory.Create<ChatsViewModel>(HostScreen),
                NavigationTarget.Account => factory.Create<AccountViewModel>(HostScreen),
                NavigationTarget.Settings => factory.Create<SettingsViewModel>(HostScreen),
                _ => Content
            };
        };
    }

    public SplitViewMenuViewModel Menu { get; }

    public ViewModelBase Content
    {
        get => _content;
        private set => this.RaiseAndSetIfChanged(ref _content, value);
    }
}