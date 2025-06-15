using Cryptie.Client.Core.Base;
using Cryptie.Client.Core.Factories;
using Cryptie.Client.Core.Navigation;
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
        Menu = menu;
        ContentCoordinator = new ContentCoordinator(this, vmFactory, HostScreen);

        Menu.ContentCoordinator = ContentCoordinator;

        Content = vmFactory.Create<ChatsViewModel>(HostScreen);
    }

    private IContentCoordinator ContentCoordinator { get; }

    public SplitViewMenuViewModel Menu { get; }

    public ViewModelBase Content
    {
        get => _content;
        set => this.RaiseAndSetIfChanged(ref _content, value);
    }
}