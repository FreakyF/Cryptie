using System;
using System.Net;
using System.Net.Http;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using Cryptie.Client.Features.ServerStatus.Services;
using static System.Threading.CancellationTokenSource;

namespace Cryptie.Client.Core.Services;

public sealed class ConnectionMonitor(IServerStatus serverStatus, TimeSpan? interval = null)
    : IConnectionMonitor, IDisposable
{
    private readonly TimeSpan _interval = interval ?? TimeSpan.FromSeconds(5);

    private readonly IServerStatus
        _serverStatus = serverStatus ?? throw new ArgumentNullException(nameof(serverStatus));

    private readonly Subject<bool> _subject = new();
    private CancellationTokenSource? _cts;
    private bool _disposed;

    public IObservable<bool> ConnectionStatusChanged => _subject;

    /// <summary>
    ///     Performs a single check against the backend service.
    /// </summary>
    /// <returns><c>true</c> when the backend responds successfully.</returns>
    public async Task<bool> IsBackendAliveAsync()
    {
        return await CheckServerAsync(CancellationToken.None);
    }

    /// <summary>
    ///     Starts monitoring the backend availability and publishes changes via <see cref="ConnectionStatusChanged"/>.
    /// </summary>
    /// <param name="token">Optional cancellation token to stop monitoring.</param>
    public void Start(CancellationToken token = default)
    {
        ThrowIfDisposed();

        if (_cts != null)
        {
            return;
        }

        _cts = CreateLinkedTokenSource(token);

        _ = Task.Run(async () =>
        {
            bool? last = null;
            while (!_cts.Token.IsCancellationRequested)
            {
                var ok = await CheckServerAsync(_cts.Token);

                if (last == null || ok != last.Value)
                {
                    _subject.OnNext(ok);
                }

                last = ok;
                await Task.Delay(_interval, _cts.Token);
            }
        }, _cts.Token);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        Stop();
        _subject.Dispose();
        _disposed = true;
    }

    private async Task<bool> CheckServerAsync(CancellationToken token)
    {
        try
        {
            await _serverStatus.GetServerStatusAsync(token);
            return true;
        }
        catch (HttpRequestException httpEx)
        {
            return httpEx.StatusCode == HttpStatusCode.TooManyRequests;
        }
        catch
        {
            return false;
        }
    }

    private void Stop()
    {
        if (_cts == null)
        {
            return;
        }

        _cts.Cancel();
        _cts.Dispose();
        _cts = null;
        _subject.OnCompleted();
    }

    private void ThrowIfDisposed()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
    }
}