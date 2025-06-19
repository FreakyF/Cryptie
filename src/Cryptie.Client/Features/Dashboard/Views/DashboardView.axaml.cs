using Avalonia.ReactiveUI;
using Cryptie.Client.Features.Dashboard.ViewModels;

namespace Cryptie.Client.Features.Dashboard.Views;

public partial class DashboardView : ReactiveUserControl<DashboardViewModel>
{
    public DashboardView()
    {
        InitializeComponent();
    }
}