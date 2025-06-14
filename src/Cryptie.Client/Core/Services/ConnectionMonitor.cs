using System;
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

    private readonly IServerStatus _serverStatus = serverStatus
                                                   ?? throw new ArgumentNullException(nameof(serverStatus));

    private readonly Subject<bool> _subject = new();
    private CancellationTokenSource? _cts;
    private bool _disposed;

    public IObservable<bool> ConnectionStatusChanged => _subject;

    public void Start(CancellationToken token = default)
    {
        ThrowIfDisposed();

        if (_cts != null) return;
        _cts = CreateLinkedTokenSource(token);

        _ = Task.Run(async () =>
        {
            bool? last = null;
            while (!_cts.Token.IsCancellationRequested)
            {
                bool ok;
                try
                {
                    await _serverStatus.GetServerStatusAsync(_cts.Token);
                    ok = true;
                }
                catch
                {
                    ok = false;
                }

                if (last == null || ok != last.Value)
                    _subject.OnNext(ok);

                last = ok;
                await Task.Delay(_interval, _cts.Token);
            }
        }, _cts.Token);
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        Stop();
        _subject.Dispose();
        _disposed = true;
    }

    private void Stop()
    {
        if (_cts == null) return;

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