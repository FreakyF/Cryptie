using Cryptie.Client.Core.Navigation;
using ReactiveUI;

namespace Cryptie.Client.Features.Shell.ViewModels;

public class MainWindowViewModel : ReactiveObject
{
    private readonly IShellCoordinator _coordinator;

    public MainWindowViewModel(IShellCoordinator coordinator)
    {
        _coordinator = coordinator;
        _coordinator.Start();
    }

    public RoutingState Router => _coordinator.Router;
}