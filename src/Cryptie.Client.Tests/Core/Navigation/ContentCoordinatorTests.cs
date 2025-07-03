using Cryptie.Client.Configuration;
using Cryptie.Client.Core.Factories;
using Cryptie.Client.Core.Navigation;
using Cryptie.Client.Core.Services;
using Cryptie.Client.Features.Account.Dependencies;
using Cryptie.Client.Features.Account.services;
using Cryptie.Client.Features.Account.ViewModels;
using Cryptie.Client.Features.AddFriend.Services;
using Cryptie.Client.Features.AddUserToGroup.ViewModels;
using Cryptie.Client.Features.Authentication.Services;
using Cryptie.Client.Features.Authentication.State;
using Cryptie.Client.Features.Chats.Dependencies;
using Cryptie.Client.Features.Chats.Services;
using Cryptie.Client.Features.Chats.ViewModels;
using Cryptie.Client.Features.ChatSettings.ViewModels;
using Cryptie.Client.Features.Dashboard.ViewModels;
using Cryptie.Client.Features.Groups.Dependencies;
using Cryptie.Client.Features.Groups.Services;
using Cryptie.Client.Features.Groups.State;
using Cryptie.Client.Features.Menu.State;
using Cryptie.Client.Features.Menu.ViewModels;
using Cryptie.Client.Features.Settings.Services;
using Cryptie.Client.Features.Settings.ViewModels;
using Cryptie.Common.Features.UserManagement.DTOs;
using Microsoft.Extensions.Options;
using Moq;
using ReactiveUI;

namespace Cryptie.Client.Tests.Core.Navigation;

public class ContentCoordinatorTests
{
    private readonly Mock<IViewModelFactory> _factoryMock = new();
    private readonly Mock<IScreen> _screenMock = new();
    private readonly Mock<IConnectionMonitor> _connectionMonitorMock = new();
    private readonly Mock<IUserState> _userStateMock = new();
    private readonly Mock<IKeychainManagerService> _keychainMock = new();
    private readonly Mock<IShellCoordinator> _shellCoordinatorMock = new();
    private readonly Mock<IAccountService> _accountServiceMock = new();
    private readonly Mock<FluentValidation.IValidator<UserDisplayNameRequestDto>> _validatorMock = new();
    private readonly Mock<IThemeService> _themeServiceMock = new();
    private readonly Mock<IUserDetailsService> _userDetailsServiceMock = new();
    private readonly Mock<IOptions<ClientOptions>> _optionsMock = new();
    private readonly Mock<IGroupService> _groupServiceMock = new();
    private readonly Mock<IGroupSelectionState> _groupSelectionStateMock = new();
    private readonly Mock<IMessagesService> _messagesServiceMock = new();
    private readonly Mock<IFriendsService> _friendsServiceMock = new();
    private readonly Mock<FluentValidation.IValidator<AddFriendRequestDto>> _addFriendValidatorMock = new();
    private readonly Mock<ILoginState> _loginStateMock = new();
    private readonly Mock<IRegistrationState> _registrationStateMock = new();
    private readonly Mock<IKeyService> _keyServiceMock = new();

    private readonly AddUserToGroupViewModel _addUserToGroupVm;
    private readonly ChatSettingsViewModel _chatSettingsVm;
    private readonly AddFriendDependencies _addFriendDeps;
    private readonly ChatsViewModelDependencies _chatsDeps;
    private readonly AccountDependencies _accountDeps;

    private readonly DashboardViewModel _dashboard;
    private readonly ContentCoordinator _coordinator;

    public ContentCoordinatorTests()
    {
        _themeServiceMock.Setup(x => x.CurrentTheme).Returns("Light");
        _themeServiceMock.Setup(x => x.AvailableThemes).Returns(new[] { "Light", "Dark" });

        _optionsMock.Setup(x => x.Value).Returns(new ClientOptions { BaseUri = "http://localhost", FontUri = "font.ttf" });

        _addUserToGroupVm = new AddUserToGroupViewModel(_screenMock.Object);
        _chatSettingsVm = new ChatSettingsViewModel(_screenMock.Object, _groupSelectionStateMock.Object, _optionsMock.Object, _addUserToGroupVm);

        _addFriendDeps = new AddFriendDependencies(_friendsServiceMock.Object, _addFriendValidatorMock.Object, _userStateMock.Object, _userDetailsServiceMock.Object, _keyServiceMock.Object);

        _chatsDeps = new ChatsViewModelDependencies(
            _optionsMock.Object,
            _addFriendDeps,
            _groupServiceMock.Object,
            _groupSelectionStateMock.Object,
            _messagesServiceMock.Object,
            _chatSettingsVm
        );

        _accountDeps = new AccountDependencies(
            _userStateMock.Object,
            _groupSelectionStateMock.Object,
            _loginStateMock.Object,
            _registrationStateMock.Object
        );

        _connectionMonitorMock.Setup(x => x.ConnectionStatusChanged).Returns(System.Reactive.Linq.Observable.Return(true));
        var menu = new SplitViewMenuViewModel(_userDetailsServiceMock.Object, _connectionMonitorMock.Object, _userStateMock.Object);
        _dashboard = new DashboardViewModel(_screenMock.Object, menu, _factoryMock.Object);
        _coordinator = new ContentCoordinator(_dashboard, _factoryMock.Object, _screenMock.Object);

        _messagesServiceMock.Setup(x => x.MessageReceived).Returns(System.Reactive.Linq.Observable.Empty<Cryptie.Client.Features.Chats.Entities.SignalRMessage>());
    }

    [Fact]
    public void ShowChats_SetsDashboardContentToChatsViewModel()
    {
        var chatsVm = new ChatsViewModel(_screenMock.Object, _connectionMonitorMock.Object, _chatsDeps, _userStateMock.Object);
        _factoryMock.Setup(f => f.Create<ChatsViewModel>(_screenMock.Object)).Returns(chatsVm);

        _coordinator.ShowChats();

        Assert.Equal(chatsVm, _dashboard.Content);
        _factoryMock.Verify(f => f.Create<ChatsViewModel>(_screenMock.Object), Times.AtLeastOnce());
    }

    [Fact]
    public void ShowAccount_SetsDashboardContentToAccountViewModel()
    {
        var accountVm = new AccountViewModel(
            _screenMock.Object,
            _keychainMock.Object,
            _shellCoordinatorMock.Object,
            _accountServiceMock.Object,
            _validatorMock.Object,
            _accountDeps);

        _factoryMock.Setup(f => f.Create<AccountViewModel>(_screenMock.Object)).Returns(accountVm);

        _coordinator.ShowAccount();

        Assert.Equal(accountVm, _dashboard.Content);
        _factoryMock.Verify(f => f.Create<AccountViewModel>(_screenMock.Object), Times.Once);
    }

    [Fact]
    public void ShowSettings_SetsDashboardContentToSettingsViewModel()
    {
        var settingsVm = new SettingsViewModel(_screenMock.Object, _themeServiceMock.Object);
        _factoryMock.Setup(f => f.Create<SettingsViewModel>(_screenMock.Object)).Returns(settingsVm);

        _coordinator.ShowSettings();

        Assert.Equal(settingsVm, _dashboard.Content);
        _factoryMock.Verify(f => f.Create<SettingsViewModel>(_screenMock.Object), Times.Once);
    }
}
