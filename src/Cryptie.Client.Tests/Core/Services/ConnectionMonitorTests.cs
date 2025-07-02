using System.Net;
using Cryptie.Client.Core.Services;
using Cryptie.Client.Features.ServerStatus.Services;
using Moq;

namespace Cryptie.Client.Tests.Core.Services;

public class ConnectionMonitorTests
{
    [Fact]
    public void Constructor_ThrowsOnNullServerStatus()
    {
        Assert.Throws<ArgumentNullException>(() => new ConnectionMonitor(null!));
    }

    [Fact]
    public void Constructor_SetsDefaultInterval()
    {
        var mock = new Mock<IServerStatus>();
        var monitor = new ConnectionMonitor(mock.Object);
        Assert.NotNull(monitor);
    }

    [Fact]
    public async Task IsBackendAliveAsync_ReturnsTrue_WhenServerOk()
    {
        var mock = new Mock<IServerStatus>();
        mock.Setup(x => x.GetServerStatusAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        var monitor = new ConnectionMonitor(mock.Object);
        var result = await monitor.IsBackendAliveAsync();
        Assert.True(result);
    }

    [Fact]
    public async Task IsBackendAliveAsync_ReturnsTrue_WhenHttpRequestExceptionWithTooManyRequests()
    {
        var mock = new Mock<IServerStatus>();
        mock.Setup(x => x.GetServerStatusAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new HttpRequestException(null, null, HttpStatusCode.TooManyRequests));
        var monitor = new ConnectionMonitor(mock.Object);
        var result = await monitor.IsBackendAliveAsync();
        Assert.True(result);
    }

    [Fact]
    public async Task IsBackendAliveAsync_ReturnsFalse_WhenHttpRequestExceptionWithOtherStatus()
    {
        var mock = new Mock<IServerStatus>();
        mock.Setup(x => x.GetServerStatusAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new HttpRequestException(null, null, HttpStatusCode.BadGateway));
        var monitor = new ConnectionMonitor(mock.Object);
        var result = await monitor.IsBackendAliveAsync();
        Assert.False(result);
    }

    [Fact]
    public async Task IsBackendAliveAsync_ReturnsFalse_OnOtherException()
    {
        var mock = new Mock<IServerStatus>();
        mock.Setup(x => x.GetServerStatusAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("fail"));
        var monitor = new ConnectionMonitor(mock.Object);
        var result = await monitor.IsBackendAliveAsync();
        Assert.False(result);
    }

    [Fact]
    public void Start_ThrowsIfDisposed()
    {
        var mock = new Mock<IServerStatus>();
        var monitor = new ConnectionMonitor(mock.Object);
        monitor.Dispose();
        Assert.Throws<ObjectDisposedException>(() => monitor.Start());
    }

    [Fact]
    public void Dispose_CanBeCalledMultipleTimes()
    {
        var mock = new Mock<IServerStatus>();
        var monitor = new ConnectionMonitor(mock.Object);
        monitor.Dispose();
        monitor.Dispose();
        Assert.True(true); 
    }

    [Fact]
    public async Task Start_EmitsStatusChange()
    {
        var mock = new Mock<IServerStatus>();
        var callCount = 0;
        mock.Setup(x => x.GetServerStatusAsync(It.IsAny<CancellationToken>()))
            .Returns(() =>
            {
                callCount++;
                if (callCount == 1) return Task.CompletedTask;
                if (callCount == 2) throw new HttpRequestException(null, null, HttpStatusCode.BadGateway);
                return Task.CompletedTask;
            });
        var monitor = new ConnectionMonitor(mock.Object, TimeSpan.FromMilliseconds(10));
        var results = new System.Collections.Generic.List<bool>();
        using var sub = monitor.ConnectionStatusChanged.Subscribe(results.Add);
        monitor.Start();
        await Task.Delay(50); 
        monitor.Dispose();
        Assert.Contains(true, results);
        Assert.Contains(false, results);
    }

    [Fact]
    public void Start_DoesNotStartTwice()
    {
        var mock = new Mock<IServerStatus>();
        var monitor = new ConnectionMonitor(mock.Object, TimeSpan.FromMilliseconds(10));
        monitor.Start();
        monitor.Start(); 
        monitor.Dispose();
        Assert.True(true);
    }
}

