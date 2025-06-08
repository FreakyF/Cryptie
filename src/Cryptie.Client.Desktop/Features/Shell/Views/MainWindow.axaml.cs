using Avalonia.ReactiveUI;
using Cryptie.Client.Desktop.Features.Shell.ViewModels;

namespace Cryptie.Client.Desktop.Features.Shell.Views;

public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
{
    public MainWindow(MainWindowViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}