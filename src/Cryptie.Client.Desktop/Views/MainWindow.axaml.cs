using System.Reactive.Disposables;
using Avalonia.ReactiveUI;
using Cryptie.Client.Desktop.ViewModels;
using ReactiveUI;

namespace Cryptie.Client.Desktop.Views;

public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
{
    public MainWindow(MainWindowViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        this.WhenActivated(disposables =>
        {
            this.OneWayBind(ViewModel, vm => vm.Router, v => v.RoutingView.Router)
                .DisposeWith(disposables);
        });
    }
}