using System.Net;
using System.Reflection;
using System.Reactive.Linq;
using Cryptie.Client.Core.Factories;
using Cryptie.Client.Core.Navigation;
using Cryptie.Client.Core.Dependencies;
using Cryptie.Client.Core.Services;
using Cryptie.Client.Features.Authentication.Services;
using Cryptie.Client.Features.Authentication.State;
using Cryptie.Common.Features.UserManagement.DTOs;
using Cryptie.Client.Features.Authentication.ViewModels;
using Cryptie.Client.Features.Dashboard.ViewModels;
using Cryptie.Client.Features.Groups.State;
using Cryptie.Client.Features.Menu.State;
using NSubstitute;
using ReactiveUI;

namespace Cryptie.Client.Tests.Core.Navigation
{
    public class ShellCoordinatorTests
    {
        private readonly IViewModelFactory _factory = Substitute.For<IViewModelFactory>();
        private readonly IKeychainManagerService _keychain = Substitute.For<IKeychainManagerService>();
        private readonly IUserDetailsService _userDetails = Substitute.For<IUserDetailsService>();
        private readonly IConnectionMonitor _connectionMonitor = Substitute.For<IConnectionMonitor>();

        private readonly ShellStateDependencies _stateDeps =
            new ShellStateDependencies(
                new UserState(),
                new GroupSelectionState(),
                new LoginState(),
                new RegistrationState()
            );

        private ShellCoordinator CreateCoordinator()
        {
            var coord = new ShellCoordinator(
                _factory,
                _keychain,
                _userDetails,
                _connectionMonitor,
                _stateDeps
            );

            // --- STUB OUT NAVIGATION SO IT NEVER BOUNCES ON IRoutableViewModel CHECK ---
            var stubNav =
                ReactiveCommand.CreateFromObservable<IRoutableViewModel, IRoutableViewModel>(vm =>
                    Observable.Return(vm));
            var stubNavReset =
                ReactiveCommand.CreateFromObservable<IRoutableViewModel, IRoutableViewModel>(vm =>
                    Observable.Return(vm));

            var router = coord.Router;
            typeof(RoutingState)
                .GetField("<Navigate>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic)!
                .SetValue(router, stubNav);
            typeof(RoutingState)
                .GetField("<NavigateAndReset>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic)!
                .SetValue(router, stubNavReset);

            return coord;
        }

        [Fact]
        public async Task StartAsync_NoSessionToken_ResetsAndShowsLogin()
        {
            _keychain.TryGetSessionToken(out _, out _)
                .ReturnsForAnyArgs(call =>
                {
                    call[0] = null;
                    call[1] = null;
                    return false;
                });

            var coord = CreateCoordinator();
            await coord.StartAsync();

            _factory.Received(1).Create<LoginViewModel>(coord);
        }

        [Fact]
        public async Task StartAsync_BackendDown_ResetsAndShowsLogin()
        {
            var token = Guid.NewGuid().ToString();
            _keychain.TryGetSessionToken(out _, out _)
                .ReturnsForAnyArgs(call =>
                {
                    call[0] = token;
                    call[1] = null;
                    return true;
                });
            _connectionMonitor.IsBackendAliveAsync().Returns(false);

            var coord = CreateCoordinator();
            await coord.StartAsync();

            _factory.Received(1).Create<LoginViewModel>(coord);
        }

        [Fact]
        public async Task StartAsync_InvalidUserGuid_ResetsAndShowsLogin()
        {
            var token = Guid.NewGuid().ToString();
            _keychain.TryGetSessionToken(out _, out _)
                .ReturnsForAnyArgs(call =>
                {
                    call[0] = token;
                    call[1] = null;
                    return true;
                });
            _connectionMonitor.IsBackendAliveAsync().Returns(true);
            _userDetails
                .GetUserGuidFromTokenAsync(Arg.Any<UserGuidFromTokenRequestDto>())
                .Returns(Task.FromResult(new UserGuidFromTokenResponseDto { Guid = Guid.Empty }));

            var coord = CreateCoordinator();
            await coord.StartAsync();

            _factory.Received(1).Create<LoginViewModel>(coord);
        }

        [Fact]
        public async Task StartAsync_MissingPrivateKey_ResetsAndShowsLogin()
        {
            var token = Guid.NewGuid().ToString();
            _keychain.TryGetSessionToken(out _, out _)
                .ReturnsForAnyArgs(call =>
                {
                    call[0] = token;
                    call[1] = null;
                    return true;
                });
            _connectionMonitor.IsBackendAliveAsync().Returns(true);
            _userDetails
                .GetUserGuidFromTokenAsync(Arg.Any<UserGuidFromTokenRequestDto>())
                .Returns(Task.FromResult(new UserGuidFromTokenResponseDto { Guid = Guid.NewGuid() }));
            _keychain.TryGetPrivateKey(out _, out _)
                .ReturnsForAnyArgs(call =>
                {
                    call[0] = null;
                    call[1] = null;
                    return false;
                });

            var coord = CreateCoordinator();
            await coord.StartAsync();

            _factory.Received(1).Create<LoginViewModel>(coord);
        }

        [Fact]
        public async Task StartAsync_AuthError_ResetsAndShowsLogin()
        {
            var token = Guid.NewGuid().ToString();
            _keychain.TryGetSessionToken(out _, out _)
                .ReturnsForAnyArgs(call =>
                {
                    call[0] = token;
                    call[1] = null;
                    return true;
                });
            _connectionMonitor.IsBackendAliveAsync().Returns(true);
            _userDetails
                .When(s => s.GetUserGuidFromTokenAsync(Arg.Any<UserGuidFromTokenRequestDto>()))
                .Do(_ => throw new HttpRequestException("denied", null, HttpStatusCode.Unauthorized));

            var coord = CreateCoordinator();
            await coord.StartAsync();

            _factory.Received(1).Create<LoginViewModel>(coord);
        }

        [Fact]
        public async Task StartAsync_AllGood_ShowsDashboard()
        {
            var token = Guid.NewGuid().ToString();
            var userGuid = Guid.NewGuid();

            _keychain.TryGetSessionToken(out _, out _)
                .ReturnsForAnyArgs(call =>
                {
                    call[0] = token;
                    call[1] = null;
                    return true;
                });
            _connectionMonitor.IsBackendAliveAsync().Returns(true);
            _userDetails
                .GetUserGuidFromTokenAsync(Arg.Any<UserGuidFromTokenRequestDto>())
                .Returns(Task.FromResult(new UserGuidFromTokenResponseDto { Guid = userGuid }));
            _keychain.TryGetPrivateKey(out _, out _)
                .ReturnsForAnyArgs(call =>
                {
                    call[0] = "dummy";
                    call[1] = null;
                    return true;
                });

            var coord = CreateCoordinator();
            await coord.StartAsync();

            _factory.Received(1).Create<DashboardViewModel>(coord);
        }

        [Fact]
        public void ShowMethods_DoNotThrow()
        {
            var coord = CreateCoordinator();
            coord.ShowLogin();
            coord.ShowRegister();
            coord.ShowQrSetup();
            coord.ShowTotpCode();
            coord.ShowDashboard();
            coord.ShowPinSetup();
        }
    }
}