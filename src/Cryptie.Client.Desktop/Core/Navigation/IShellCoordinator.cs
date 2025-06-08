using ReactiveUI;



public interface IShellCoordinator : IScreen
{
    void Start();
    void ShowLogin();
    void ShowRegister();
    void ShowQrSetup();
    void ShowTotpCode();
    void ShowDashboard();
}