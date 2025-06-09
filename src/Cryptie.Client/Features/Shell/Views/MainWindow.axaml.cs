using Avalonia.ReactiveUI;
using Cryptie.Client.Features.Shell.ViewModels;

namespace Cryptie.Client.Features.Shell.Views;

public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
{
    // ReSharper disable once MemberCanBePrivate.Global
    public MainWindow()
    {
        InitializeComponent();
    }

    public MainWindow(MainWindowViewModel viewModel) : this()
    {
        DataContext = viewModel;
    }
}