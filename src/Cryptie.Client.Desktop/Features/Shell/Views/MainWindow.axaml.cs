using Avalonia.ReactiveUI;
using Cryptie.Client.Desktop.Features.Shell.ViewModels;



public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
{
    public MainWindow(MainWindowViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}