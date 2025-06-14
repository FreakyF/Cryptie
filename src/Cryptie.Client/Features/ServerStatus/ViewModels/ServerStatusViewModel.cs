using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Cryptie.Client.Core.Base;
using Cryptie.Client.Features.ServerStatus.Services;
using ReactiveUI;

namespace Cryptie.Client.Features.ServerStatus.ViewModels;

public sealed class ServerStatusViewModel : RoutableViewModelBase, INotifyPropertyChanged, IDisposable
{
    private readonly CancellationTokenSource _cts = new();
    private readonly IServerStatus _serverStatus;

    private bool _isLoading = true;

    public ServerStatusViewModel(IScreen hostScreen, IServerStatus serverStatus)
        : base(hostScreen)
    {
        _serverStatus = serverStatus;
        _ = InitializeAsync(_cts.Token);
    }

    public bool IsLoading
    {
        get => _isLoading;
        private set
        {
            if (_isLoading == value) return;
            _isLoading = value;
            OnPropertyChanged();
        }
    }

    public void Dispose()
    {
        if (!_cts.IsCancellationRequested)
            _cts.Cancel();
        _cts.Dispose();
    }

    public new event PropertyChangedEventHandler? PropertyChanged;

    private async Task InitializeAsync(CancellationToken ct)
    {
        await RetryAsync(TimeSpan.FromSeconds(1), 5, ct);
        await RetryAsync(TimeSpan.FromSeconds(10), 5, ct);
        await RetryUntilOkAsync(TimeSpan.FromSeconds(30), ct);

        IsLoading = false;

        await MonitorLoopAsync(ct);
    }

    private async Task MonitorLoopAsync(CancellationToken ct)
    {
        while (true)
        {
            ct.ThrowIfCancellationRequested();

            try
            {
                await _serverStatus.GetServerStatusAsync(ct);
                await Task.Delay(TimeSpan.FromSeconds(30), ct);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch
            {
                await RetryAsync(TimeSpan.FromSeconds(10), 5, ct);
                await RetryUntilOkAsync(TimeSpan.FromSeconds(30), ct);
            }
        }
    }

    private async Task RetryAsync(TimeSpan delay, int maxAttempts, CancellationToken ct)
    {
        for (var i = 0; i < maxAttempts; i++)
        {
            ct.ThrowIfCancellationRequested();

            try
            {
                await _serverStatus.GetServerStatusAsync(ct);
                return;
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch
            {
                await Task.Delay(delay, ct);
            }
        }
    }

    private async Task RetryUntilOkAsync(TimeSpan delay, CancellationToken ct)
    {
        while (true)
        {
            ct.ThrowIfCancellationRequested();

            try
            {
                await _serverStatus.GetServerStatusAsync(ct);
                return;
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch
            {
                await Task.Delay(delay, ct);
            }
        }
    }

    private void OnPropertyChanged([CallerMemberName] string? name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}