using Cryptie.Client.Desktop.Composition.Factories;
using ReactiveUI;

namespace Cryptie.Client.Desktop.ViewModels;

public class MainWindowViewModel : ReactiveObject
{
    public RoutingState Router => _coordinator.Router;
    private readonly IAppCoordinator _coordinator;

    public MainWindowViewModel(IAppCoordinator coordinator)
    {
        _coordinator = coordinator;
        _coordinator.Start();
    }
}