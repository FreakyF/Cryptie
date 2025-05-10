using ReactiveUI;

namespace Cryptie.Client.Desktop.Core.Navigation;

public interface IShellCoordinator : IScreen
{
    void Start();
    void ShowLogin();
    void ShowRegister();
}