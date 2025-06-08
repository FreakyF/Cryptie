using Avalonia.ReactiveUI;
using Cryptie.Client.Desktop.Features.Authentication.ViewModels;



public partial class DashboardView : ReactiveUserControl<DashboardViewModel>
{
    public DashboardView()
    {
        InitializeComponent();
    }
}