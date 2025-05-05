using Cryptie.Client.Desktop.Coordinators;
using ReactiveUI;

namespace Cryptie.Client.Desktop.ViewModels;

public class MainWindowViewModel : ReactiveObject
{
    private readonly IAppCoordinator _coordinator;

    public MainWindowViewModel(IAppCoordinator coordinator)
    {
        _coordinator = coordinator;
        _coordinator.Start();
    }

    public RoutingState Router => _coordinator.Router;
}