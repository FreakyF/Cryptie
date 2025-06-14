using System;
using System.Reactive.Linq;
using Avalonia.ReactiveUI;
using Cryptie.Client.Core.Factories;
using Cryptie.Client.Core.Navigation;
using Cryptie.Client.Core.Services;
using Cryptie.Client.Features.ServerStatus.ViewModels;
using ReactiveUI;

namespace Cryptie.Client.Features.Shell.ViewModels;

public sealed class MainWindowViewModel : ReactiveObject, IScreen, IDisposable
{
    private readonly IConnectionMonitor _connectionMonitor;

    private readonly IDisposable _connectionSubscription;
    private readonly IViewModelFactory _factory;
    private readonly IShellCoordinator _shellCoordinator;
    private bool _disposed;

    private bool _isShowingStatus;
    private IDisposable? _statusVmIsLoadingSub;

    public MainWindowViewModel(
        IShellCoordinator shellCoordinator,
        IConnectionMonitor connectionMonitor,
        IViewModelFactory factory)
    {
        _shellCoordinator = shellCoordinator ?? throw new ArgumentNullException(nameof(shellCoordinator));
        _connectionMonitor = connectionMonitor ?? throw new ArgumentNullException(nameof(connectionMonitor));
        _factory = factory ?? throw new ArgumentNullException(nameof(factory));

        _shellCoordinator.Start();

        _connectionSubscription = _connectionMonitor.ConnectionStatusChanged
            .StartWith(false)
            .ObserveOn(AvaloniaScheduler.Instance)
            .Subscribe(ok =>
            {
                if (ok) OnConnectionRestored();
                else OnConnectionLost();
            });

        _connectionMonitor.Start();
    }

    public void Dispose()
    {
        if (_disposed) return;
        Stop();
        _disposed = true;
    }

    public RoutingState Router => _shellCoordinator.Router;

    private void OnConnectionLost()
    {
        ThrowIfDisposed();

        if (_isShowingStatus) return;
        _isShowingStatus = true;

        var statusVm = _factory.Create<ServerStatusViewModel>(this);
        Router.Navigate.Execute(statusVm);

        _statusVmIsLoadingSub?.Dispose();
        _statusVmIsLoadingSub = statusVm
            .WhenAnyValue(vm => vm.IsLoading)
            .ObserveOn(AvaloniaScheduler.Instance)
            .Where(isLoading => !isLoading)
            .Take(1)
            .Subscribe(_ => OnConnectionRestored());
    }

    private void OnConnectionRestored()
    {
        ThrowIfDisposed();

        if (!_isShowingStatus) return;
        _isShowingStatus = false;

        _statusVmIsLoadingSub?.Dispose();
        _statusVmIsLoadingSub = null;

        Router.NavigateBack.Execute();
    }

    private void Stop()
    {
        _connectionSubscription.Dispose();
        if (_connectionMonitor is IDisposable d)
            d.Dispose();
    }

    private void ThrowIfDisposed()
        => ObjectDisposedException.ThrowIf(_disposed, this);
}