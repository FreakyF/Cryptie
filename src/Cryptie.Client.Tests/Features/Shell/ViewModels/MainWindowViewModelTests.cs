using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using Cryptie.Client.Core.Factories;
using Cryptie.Client.Core.Navigation;
using Cryptie.Client.Core.Services;
using Cryptie.Client.Features.ServerStatus.ViewModels;
using Cryptie.Client.Features.Shell.ViewModels;
using NSubstitute;
using ReactiveUI;

namespace Cryptie.Client.Tests.Features.Shell.ViewModels
{
    public class MainWindowViewModelTests
    {
        private readonly IShellCoordinator _shell = Substitute.For<IShellCoordinator>();
        private readonly IConnectionMonitor _connMon = Substitute.For<IConnectionMonitor>();
        private readonly IViewModelFactory _factory = Substitute.For<IViewModelFactory>();
        private readonly RoutingState _router = new();
        private MainWindowViewModel _vm;

        private int _navCount;
        private int _navResetCount;

        public MainWindowViewModelTests()
        {
            var stubNav = ReactiveCommand.CreateFromObservable<IRoutableViewModel, IRoutableViewModel>(vm =>
            {
                _navCount++;
                return Observable.Return(vm);
            });
            var stubNavReset = ReactiveCommand.CreateFromObservable<IRoutableViewModel, IRoutableViewModel>(vm =>
            {
                _navResetCount++;
                return Observable.Return(vm);
            });

            typeof(RoutingState)
                .GetField("<Navigate>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic)!
                .SetValue(_router, stubNav);
            typeof(RoutingState)
                .GetField("<NavigateAndReset>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic)!
                .SetValue(_router, stubNavReset);

            _shell.Router.Returns(_router);
            _shell.StartAsync().Returns(Task.CompletedTask);

            _connMon.ConnectionStatusChanged.Returns(Observable.Never<bool>());
            _connMon.When(x => x.Start()).Do(_ => { });

            var dummyStatusVm = (ServerStatusViewModel)
                FormatterServices.GetUninitializedObject(typeof(ServerStatusViewModel));
            _factory
                .Create<ServerStatusViewModel>(Arg.Any<IScreen>())
                .Returns(dummyStatusVm);

            _vm = new MainWindowViewModel(_shell, _connMon, _factory);
        }

        [Fact]
        public void Ctor_CallsShellStart_Twice_And_ConnectionMonitorStart_Once()
        {
            _shell.Received(2).StartAsync();
            _connMon.Received(1).Start();

            Assert.Equal(1, _navCount);
            Assert.Equal(0, _navResetCount);
        }

        [Fact]
        public void OnConnectionLost_CreatesServerStatusVm_And_NavigatesOnceOnly()
        {
            _navCount = 0;
            _factory.ClearReceivedCalls();

            var onLost = typeof(MainWindowViewModel)
                .GetMethod("OnConnectionLost", BindingFlags.Instance | BindingFlags.NonPublic)!;

            onLost.Invoke(_vm, null);
            Assert.Equal(1, _navCount);
            _factory.Received(1).Create<ServerStatusViewModel>(_vm);

            onLost.Invoke(_vm, null);
            Assert.Equal(2, _navCount);
        }

        [Fact]
        public async Task HandleConnectionRestoredAsync_WhenNotShowingStatus_DoesNothing_BeyondCtorCalls()
        {
            var handleM = typeof(MainWindowViewModel)
                .GetMethod("HandleConnectionRestoredAsync", BindingFlags.Instance | BindingFlags.NonPublic)!;
            var result = handleM.Invoke(_vm, null);
            var task = (Task)result!;
            await task;

            _shell.Received(2).StartAsync();
        }

        [Fact]
        public async Task HandleConnectionRestoredAsync_WhenShowingStatus_TripsAgain()
        {
            var isShowingField = typeof(MainWindowViewModel)
                .GetField("_isShowingStatus", BindingFlags.Instance | BindingFlags.NonPublic)!;
            isShowingField.SetValue(_vm, true);

            var subField = typeof(MainWindowViewModel)
                .GetField("_statusVmIsLoadingSub", BindingFlags.Instance | BindingFlags.NonPublic)!;
            subField.SetValue(_vm, Disposable.Empty);

            var handleM = typeof(MainWindowViewModel)
                .GetMethod("HandleConnectionRestoredAsync", BindingFlags.Instance | BindingFlags.NonPublic)!;
            var result = handleM.Invoke(_vm, null);
            var task = (Task)result!;
            await task;

            _shell.Received(3).StartAsync();
            Assert.False((bool)isShowingField.GetValue(_vm)!);
        }

        [Fact]
        public void Dispose_CanBeCalledMultipleTimes_And_NoException()
        {
            _vm.Dispose();
            _vm.Dispose();
        }

        [Fact]
        public void OnConnectionLost_AfterDispose_ThrowsObjectDisposedException()
        {
            _vm.Dispose();
            var onLost = typeof(MainWindowViewModel)
                .GetMethod("OnConnectionLost", BindingFlags.Instance | BindingFlags.NonPublic)!;

            var ex = Assert.Throws<TargetInvocationException>(() => onLost.Invoke(_vm, null));
            Assert.IsType<ObjectDisposedException>(ex.InnerException);
        }
    }
}