// using System;
// using System.Collections.ObjectModel;
// using System.Reactive;
// using System.Reactive.Linq;
// using Cryptie.Client.Features.Chats.Dependencies;
// using Cryptie.Client.Features.Chats.ViewModels;
// using Cryptie.Client.Features.Groups.ViewModels;
// using Cryptie.Client.Features.ChatSettings.ViewModels;
// using Cryptie.Client.Features.Groups.State;
// using Cryptie.Client.Core.Services;
// using Cryptie.Client.Configuration;
// using Cryptie.Client.Features.Chats.Services;
// using Cryptie.Client.Features.Groups.Services;
// using Cryptie.Client.Features.Menu.State;
// using Cryptie.Common.Features.Messages.DTOs;
// using Microsoft.Extensions.Options;
// using Moq;
// using ReactiveUI;
// using Xunit;
//
// namespace Cryptie.Client.Tests.Features.Chats.ViewModels;
//
// public class ChatsViewModelTests
// {
//     private readonly Mock<IScreen> _screenMock = new();
//     private readonly Mock<IConnectionMonitor> _connectionMonitorMock = new();
//     private readonly Mock<IUserState> _userStateMock = new();
//     private readonly Mock<IGroupSelectionState> _groupStateMock = new();
//     private readonly Mock<IGroupService> _groupServiceMock = new();
//     private readonly Mock<IMessagesService> _messagesServiceMock = new();
//     private readonly Mock<IOptions<ClientOptions>> _optionsMock = new();
//     private readonly Mock<IKeyService> _keyServiceMock = new();
//     private readonly Mock<IAddFriendService> _addFriendServiceMock = new();
//     private readonly Mock<IAddFriendDialogService> _addFriendDialogServiceMock = new();
//     private readonly Mock<IAddFriendValidator> _addFriendValidatorMock = new();
//     private readonly Mock<IAddFriendDependencies> _addFriendDependenciesMock = new();
//     private readonly Mock<ChatSettingsViewModel> _settingsPanelMock = new();
//     private readonly GroupsListViewModel _groupsPanel;
//     private readonly ChatsViewModelDependencies _deps;
//
//     public ChatsViewModelTests()
//     {
//         _groupStateMock.SetupGet(x => x.SelectedGroupId).Returns(Guid.NewGuid());
//         _groupStateMock.SetupGet(x => x.SelectedGroupName).Returns("TestGroup");
//         _messagesServiceMock.SetupGet(x => x.MessageReceived).Returns(Observable.Never<GroupMessageSignalDto>());
//         _addFriendDependenciesMock.SetupGet(x => x.KeyService).Returns(_keyServiceMock.Object);
//         _deps = new ChatsViewModelDependencies
//         {
//             Options = _optionsMock.Object,
//             AddFriendDependencies = _addFriendDependenciesMock.Object,
//             GroupService = _groupServiceMock.Object,
//             MessagesService = _messagesServiceMock.Object,
//             GroupState = _groupStateMock.Object,
//             SettingsPanel = _settingsPanelMock.Object
//         };
//         _groupsPanel = new GroupsListViewModel(
//             _screenMock.Object,
//             _connectionMonitorMock.Object,
//             _optionsMock.Object,
//             _addFriendDependenciesMock.Object,
//             _groupServiceMock.Object,
//             _messagesServiceMock.Object,
//             _groupStateMock.Object,
//             _keyServiceMock.Object
//         );
//     }
//
//     [Fact]
//     public void Constructor_InitializesProperties()
//     {
//         // Arrange
//         _userStateMock.SetupGet(x => x.SessionToken).Returns(Guid.NewGuid().ToString());
//         _userStateMock.SetupGet(x => x.UserId).Returns(Guid.NewGuid());
//
//         // Act
//         var vm = new ChatsViewModel(
//             _screenMock.Object,
//             _connectionMonitorMock.Object,
//             _deps,
//             _userStateMock.Object
//         );
//
//         // Assert
//         Assert.NotNull(vm.GroupsPanel);
//         Assert.NotNull(vm.Messages);
//         Assert.NotNull(vm.SettingsPanel);
//         Assert.NotNull(vm.SendMessageCommand);
//         Assert.NotNull(vm.ToggleChatSettingsCommand);
//         Assert.False(vm.IsChatSettingsOpen);
//     }
//
//     [Fact]
//     public void MessageText_Property_Works()
//     {
//         var vm = CreateViewModel();
//         vm.MessageText = "test";
//         Assert.Equal("test", vm.MessageText);
//     }
//
//     [Fact]
//     public void IsChatSettingsOpen_Property_Works()
//     {
//         var vm = CreateViewModel();
//         vm.IsChatSettingsOpen = true;
//         Assert.True(vm.IsChatSettingsOpen);
//     }
//
//     [Fact]
//     public void ToggleChatSettingsCommand_Toggles_IsChatSettingsOpen()
//     {
//         var vm = CreateViewModel();
//         Assert.False(vm.IsChatSettingsOpen);
//         vm.ToggleChatSettingsCommand.Execute().Subscribe();
//         Assert.True(vm.IsChatSettingsOpen);
//         vm.ToggleChatSettingsCommand.Execute().Subscribe();
//         Assert.False(vm.IsChatSettingsOpen);
//     }
//
//     [Fact]
//     public void HasGroups_And_HasNoGroups_Work()
//     {
//         var vm = CreateViewModel();
//         Assert.False(vm.HasGroups);
//         Assert.True(vm.HasNoGroups);
//         vm.GroupsPanel.Groups.Add(new GroupViewModel());
//         Assert.True(vm.HasGroups);
//         Assert.False(vm.HasNoGroups);
//     }
//
//     [Fact]
//     public void Dispose_Disposes_Disposables()
//     {
//         var vm = CreateViewModel();
//         vm.Dispose();
//         // No exception means success
//     }
//
//     private ChatsViewModel CreateViewModel()
//     {
//         _userStateMock.SetupGet(x => x.SessionToken).Returns(Guid.NewGuid().ToString());
//         _userStateMock.SetupGet(x => x.UserId).Returns(Guid.NewGuid());
//         return new ChatsViewModel(
//             _screenMock.Object,
//             _connectionMonitorMock.Object,
//             _deps,
//             _userStateMock.Object
//         );
//     }
// }