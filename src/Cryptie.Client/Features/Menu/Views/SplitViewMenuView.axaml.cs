using Avalonia.ReactiveUI;
using Cryptie.Client.Features.Menu.ViewModels;

namespace Cryptie.Client.Features.Menu.Views;

public partial class SplitViewMenuView : ReactiveUserControl<SplitViewMenuViewModel>
{
    public SplitViewMenuView()
    {
        InitializeComponent();
    }
}