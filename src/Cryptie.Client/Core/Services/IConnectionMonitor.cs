using System;
using System.Threading;
using System.Threading.Tasks;

namespace Cryptie.Client.Core.Services;

public interface IConnectionMonitor
{
    IObservable<bool> ConnectionStatusChanged { get; }
    public Task<bool> IsBackendAliveAsync();
    void Start(CancellationToken token = default);
}