using System;
using System.Threading;

namespace Cryptie.Client.Core.Services;

public interface IConnectionMonitor
{
    IObservable<bool> ConnectionStatusChanged { get; }
    void Start(CancellationToken token = default);
}