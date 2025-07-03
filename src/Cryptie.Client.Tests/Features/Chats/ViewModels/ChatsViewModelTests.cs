using System.Reactive.Linq;
using Cryptie.Client.Features.Chats.Dependencies;
using Cryptie.Client.Features.Chats.ViewModels;
using Cryptie.Client.Features.Groups.ViewModels;
using Cryptie.Client.Features.ChatSettings.ViewModels;
using Cryptie.Client.Features.Groups.State;
using Cryptie.Client.Core.Services;
using Cryptie.Client.Configuration;
using Cryptie.Client.Features.AddFriend.Services;
using Cryptie.Client.Features.AddUserToGroup.ViewModels;
using Cryptie.Client.Features.Chats.Entities;
using Cryptie.Client.Features.Chats.Services;
using Cryptie.Client.Features.Groups.Services;
using Cryptie.Client.Features.Menu.State;
using Microsoft.Extensions.Options;
using Moq;
using ReactiveUI;
using Cryptie.Client.Features.Groups.Dependencies;

namespace Cryptie.Client.Tests.Features.Chats.ViewModels
{
    public class ChatsViewModelTests
    {
        private readonly Mock<IScreen> _screenMock = new();
        private readonly Mock<IConnectionMonitor> _connectionMonitorMock = new();
        private readonly Mock<IUserState> _userStateMock = new();
        private readonly Mock<IGroupSelectionState> _groupStateMock = new();
        private readonly Mock<IGroupService> _groupServiceMock = new();
        private readonly Mock<IMessagesService> _messagesServiceMock = new();
        private readonly Mock<IOptions<ClientOptions>> _optionsMock = new();
        private readonly Mock<IKeyService> _keyServiceMock = new();
        private readonly ChatSettingsViewModel _settingsPanel;
        private readonly ChatsViewModelDependencies _deps;
        private readonly Mock<IFriendsService> _friendsServiceMock;
        private readonly Mock<FluentValidation.IValidator<Cryptie.Common.Features.UserManagement.DTOs.AddFriendRequestDto>> _validatorMock;
        private readonly Mock<IUserDetailsService> _userDetailsServiceMock;
        private readonly AddFriendDependencies _addFriendDependencies;
        private readonly AddUserToGroupViewModel _addUserToGroupViewModel;

        public ChatsViewModelTests()
        {
            _friendsServiceMock = new();
            _validatorMock = new();
            _userDetailsServiceMock = new();
            
            var clientOptions = new ClientOptions { BaseUri = "https://test.com", FontUri = "avares://Cryptie/Assets/Fonts" };
            _optionsMock.Setup(x => x.Value).Returns(clientOptions);
            
            _connectionMonitorMock.Setup(x => x.ConnectionStatusChanged).Returns(Observable.Never<bool>());
            
            _addFriendDependencies = new AddFriendDependencies(
                _friendsServiceMock.Object,
                _validatorMock.Object,
                _userStateMock.Object,
                _userDetailsServiceMock.Object,
                _keyServiceMock.Object
            );

            _addUserToGroupViewModel = new AddUserToGroupViewModel(_screenMock.Object);

            _settingsPanel = new ChatSettingsViewModel(
                _screenMock.Object,
                _groupStateMock.Object,
                _optionsMock.Object,
                _addUserToGroupViewModel
            );

            _groupStateMock.SetupGet(x => x.SelectedGroupId).Returns(Guid.NewGuid());
            _groupStateMock.SetupGet(x => x.SelectedGroupName).Returns("TestGroup");

            _messagesServiceMock.SetupGet(x => x.MessageReceived).Returns(Observable.Never<SignalRMessage>());

            _deps = new ChatsViewModelDependencies(
                _optionsMock.Object,
                _addFriendDependencies,
                _groupServiceMock.Object,
                _groupStateMock.Object,
                _messagesServiceMock.Object,
                _settingsPanel
            );

            new GroupsListViewModel(
                _screenMock.Object,
                _connectionMonitorMock.Object,
                _optionsMock.Object,
                _addFriendDependencies,
                _groupServiceMock.Object,
                _messagesServiceMock.Object,
                _groupStateMock.Object,
                _keyServiceMock.Object
            );
        }

        [Fact]
        public void Constructor_InitializesProperties()
        {
            _userStateMock.SetupGet(x => x.SessionToken).Returns(Guid.NewGuid().ToString());
            _userStateMock.SetupGet(x => x.UserId).Returns(Guid.NewGuid());

            var vm = new ChatsViewModel(
                _screenMock.Object,
                _connectionMonitorMock.Object,
                _deps,
                _userStateMock.Object
            );

            Assert.NotNull(vm.GroupsPanel);
            Assert.NotNull(vm.Messages);
            Assert.NotNull(vm.SettingsPanel);
            Assert.NotNull(vm.SendMessageCommand);
            Assert.NotNull(vm.ToggleChatSettingsCommand);
            Assert.False(vm.IsChatSettingsOpen);
        }

        [Fact]
        public void MessageText_Property_Works()
        {
            var vm = CreateViewModel();
            vm.MessageText = "test";
            Assert.Equal("test", vm.MessageText);
        }

        [Fact]
        public void IsChatSettingsOpen_Property_Works()
        {
            var vm = CreateViewModel();
            vm.IsChatSettingsOpen = true;
            Assert.True(vm.IsChatSettingsOpen);
        }

        [Fact]
        public void ToggleChatSettingsCommand_Toggles_IsChatSettingsOpen()
        {
            var vm = CreateViewModel();
            Assert.False(vm.IsChatSettingsOpen);
            vm.ToggleChatSettingsCommand.Execute().Subscribe();
            Assert.True(vm.IsChatSettingsOpen);
            vm.ToggleChatSettingsCommand.Execute().Subscribe();
            Assert.False(vm.IsChatSettingsOpen);
        }

        [Fact]
        public void HasGroups_And_HasNoGroups_Work()
        {
            var vm = CreateViewModel();
            Assert.False(vm.HasGroups);
            Assert.True(vm.HasNoGroups);
            vm.GroupsPanel.Groups.Add("TestGroup");
            Assert.True(vm.HasGroups);
            Assert.False(vm.HasNoGroups);
        }

        private ChatsViewModel CreateViewModel()
        {
            _userStateMock.SetupGet(x => x.SessionToken).Returns(Guid.NewGuid().ToString());
            _userStateMock.SetupGet(x => x.UserId).Returns(Guid.NewGuid());

            return new ChatsViewModel(
                _screenMock.Object,
                _connectionMonitorMock.Object,
                _deps,
                _userStateMock.Object
            );
        }


        [Fact]
        public void DecryptMessages_DoesNothing_WhenKeyDoesNotExist()
        {
            // Arrange
            var groupId = Guid.NewGuid();
            var plainText = "original";
            var encrypted = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(plainText));
            var messages = new List<Cryptie.Common.Features.Messages.DTOs.GetGroupMessagesResponseDto.MessageDto>
            {
                new() { Message = encrypted }
            };
            var vm = CreateViewModel();

            // Act
            vm.GetType().GetMethod("DecryptMessages", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.Invoke(vm, new object[] { messages, groupId });

            // Assert
            Assert.Equal(encrypted, messages[0].Message);
        }

        [Fact]
        public void DecryptMessages_HandlesEmptyMessages()
        {
            // Arrange
            var groupId = Guid.NewGuid();
            var vm = CreateViewModel();
            var groupKeyCacheField = vm.GroupsPanel.GetType().GetField("_groupKeyCache", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var dict = groupKeyCacheField?.GetValue(vm.GroupsPanel) as Dictionary<Guid, string>;
            dict![groupId] = "key";
            var messages = new List<Cryptie.Common.Features.Messages.DTOs.GetGroupMessagesResponseDto.MessageDto>();

            // Act
            vm.GetType().GetMethod("DecryptMessages", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.Invoke(vm, new object[] { messages, groupId });

            // Assert
            Assert.Empty(messages);
        }
    }
}
