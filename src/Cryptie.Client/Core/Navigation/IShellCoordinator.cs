using System.Threading.Tasks;
using ReactiveUI;

namespace Cryptie.Client.Core.Navigation;

public interface IShellCoordinator : IScreen
{
    Task StartAsync();
    void ShowLogin();
    void ResetAndShowLogin();
    void ShowRegister();
    void ShowQrSetup();
    void ShowTotpCode();
    void ShowDashboard();
    void ShowPinSetup();
}