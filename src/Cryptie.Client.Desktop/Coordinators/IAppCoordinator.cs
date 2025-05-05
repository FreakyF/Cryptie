using ReactiveUI;

namespace Cryptie.Client.Desktop.Coordinators;

public interface IAppCoordinator : IScreen
{
    void Start();
    void ShowLogin();
    void ShowRegister();
}