using Avalonia.ReactiveUI;
using Cryptie.Client.Features.Dashboard.ViewModels;

namespace Cryptie.Client.Features.Dashboard.Views;

// ReSharper disable once UnusedType.Global
public partial class DashboardView : ReactiveUserControl<DashboardViewModel>
{
    public DashboardView()
    {
        InitializeComponent();
    }
}