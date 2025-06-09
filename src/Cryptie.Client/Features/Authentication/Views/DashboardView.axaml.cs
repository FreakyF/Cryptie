using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.ReactiveUI;
using Cryptie.Client.Features.Authentication.ViewModels;

namespace Cryptie.Client.Features.Authentication.Views;

public partial class DashboardView : ReactiveUserControl<DashboardViewModel>
{
    public DashboardView()
    {
        InitializeComponent();
        AttachedToVisualTree += (_, _) => OpenHeaderWindow();
    }

    private void OpenHeaderWindow()
    {
        var header = new SelectableTextBlock
        {
            FontSize = 16,
            FontWeight = FontWeight.Bold,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new Thickness(10)
        };
        header.Bind(
            TextBlock.TextProperty,
            new Binding(nameof(DashboardViewModel.CurrentUserDetails)) { Mode = BindingMode.OneWay });

        var win = new Window
        {
            Title = "DEBUG INFO",
            SizeToContent = SizeToContent.WidthAndHeight,
            Content = header,
            DataContext = DataContext,
            CanResize = false
        };

        win.Show();
    }
}