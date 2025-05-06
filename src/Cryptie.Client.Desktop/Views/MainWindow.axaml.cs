using Avalonia.ReactiveUI;
using Cryptie.Client.Desktop.ViewModels;

namespace Cryptie.Client.Desktop.Views;

public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
{
    public MainWindow(MainWindowViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}