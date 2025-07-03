using System.Reflection;
using Cryptie.Client.Features.ServerStatus.Services;
using Cryptie.Client.Features.ServerStatus.ViewModels;
using NSubstitute;
using ReactiveUI;

namespace Cryptie.Client.Tests.Features.ServerStatus
{
    public class ServerStatusViewModelTests
    {
        private class DelayedSuccessServerStatus : IServerStatus
        {
            public async Task GetServerStatusAsync(CancellationToken ct)
            {
                await Task.Delay(50, ct);
            }
        }

        private class ImmediateSuccessServerStatus : IServerStatus
        {
            public Task GetServerStatusAsync(CancellationToken ct) => Task.CompletedTask;
        }

        private class FailingThenSuccessfulServerStatus : IServerStatus
        {
            private int _failures;
            public FailingThenSuccessfulServerStatus(int failures) => _failures = failures;

            public Task GetServerStatusAsync(CancellationToken ct)
            {
                if (_failures-- > 0) throw new Exception("transient");
                return Task.CompletedTask;
            }
        }

        [Fact]
        public async Task Constructor_RunsInitialization_And_FlipsIsLoadingFalse()
        {
            var serverStatus = new DelayedSuccessServerStatus();
            var vm = new ServerStatusViewModel(Substitute.For<IScreen>(), serverStatus);

            Assert.True(vm.IsLoading);

            var tcs = new TaskCompletionSource<bool>();
            vm.PropertyChanged += (_, e) =>
            {
                if (e.PropertyName == nameof(vm.IsLoading))
                    tcs.TrySetResult(vm.IsLoading);
            };

            var completed = await Task.WhenAny(tcs.Task, Task.Delay(1000));
            Assert.True(completed == tcs.Task, "Did not observe IsLoading change");
            Assert.False(tcs.Task.Result);

            vm.Dispose();
        }

        [Fact]
        public void Dispose_CancelsTheInternalTokenSource_And_Idempotent()
        {
            var serverStatus = new ImmediateSuccessServerStatus();
            var vm = new ServerStatusViewModel(Substitute.For<IScreen>(), serverStatus);

            var ctsField = typeof(ServerStatusViewModel)
                .GetField("_cts", BindingFlags.Instance | BindingFlags.NonPublic)!;
            var cts = (CancellationTokenSource)ctsField.GetValue(vm)!;

            Assert.False(cts.IsCancellationRequested);
            vm.Dispose();
            Assert.True(cts.IsCancellationRequested);
            vm.Dispose();
        }

        [Fact]
        public async Task Initialization_RetriesOnTransientFailures_BeforeSucceeding()
        {
            var serverStatus = new FailingThenSuccessfulServerStatus(3);
            var vm = new ServerStatusViewModel(Substitute.For<IScreen>(), serverStatus);

            var tcs = new TaskCompletionSource<bool>();
            vm.PropertyChanged += (_, e) =>
            {
                if (e.PropertyName == nameof(vm.IsLoading))
                    tcs.TrySetResult(vm.IsLoading);
            };

            var completed = await Task.WhenAny(tcs.Task, Task.Delay(5000));
            Assert.True(completed == tcs.Task, "Did not observe IsLoading change after retries");
            Assert.False(tcs.Task.Result);

            vm.Dispose();
        }
    }
}