using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.ReactiveUI;
using Cryptie.Client.Features.ServerStatus.ViewModels;

namespace Cryptie.Client.Features.ServerStatus.Views;

// ReSharper disable once UnusedType.Global
public partial class ServerStatusView : ReactiveUserControl<ServerStatusViewModel>
{
    private CancellationTokenSource? _dotAnimationCts;

    public ServerStatusView()
    {
        InitializeComponent();
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    private void OnLoaded(object? sender, RoutedEventArgs e)
    {
        _dotAnimationCts = new CancellationTokenSource();
        _ = AnimateDotsAsync(DotAnimation, _dotAnimationCts.Token);
    }

    private void OnUnloaded(object? sender, RoutedEventArgs e)
    {
        _dotAnimationCts?.Cancel();
        _dotAnimationCts?.Dispose();
    }

    private static async Task AnimateDotsAsync(TextBlock dotAnimation, CancellationToken token)
    {
        var dotStates = new[] { "", ".", "..", "..." };
        var index = 0;

        while (!token.IsCancellationRequested)
        {
            dotAnimation.Text = dotStates[index];
            index = (index + 1) % dotStates.Length;
            await Task.Delay(500, token);
        }
    }
}