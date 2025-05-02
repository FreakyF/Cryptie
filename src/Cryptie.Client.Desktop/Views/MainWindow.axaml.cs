using Avalonia.Controls;
using Cryptie.Client.Desktop.ViewModels;

namespace Cryptie.Client.Desktop.Views;

public partial class MainWindow : Window
{
    public MainWindow(MainWindowViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}