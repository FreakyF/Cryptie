using ReactiveUI;

namespace Cryptie.Client.Core.Navigation;

public interface IShellCoordinator : IScreen
{
    void Start();
    void ShowLogin();
    void ShowRegister();
    void ShowQrSetup();
    void ShowTotpCode();
    void ShowDashboard();
    void ShowChats();
    void ShowAccount();
    void ShowSettings();
}