using ReactiveUI;

namespace Cryptie.Client.Desktop.Composition.Factories;

public interface IAppCoordinator : IScreen
{
    void Start();
    void ShowLogin();
    void ShowRegister();
}