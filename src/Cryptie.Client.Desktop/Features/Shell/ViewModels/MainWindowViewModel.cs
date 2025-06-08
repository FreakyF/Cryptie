using Cryptie.Client.Desktop.Core.Navigation;
using ReactiveUI;

namespace Cryptie.Client.Desktop.Features.Shell.ViewModels;

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