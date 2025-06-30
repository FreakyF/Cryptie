// using System;
// using System.Collections.ObjectModel;
// using System.Linq;
// using System.Reactive;
// using System.Reactive.Linq;
// using System.Threading.Tasks;
// using Cryptie.Client.Configuration;
// using Cryptie.Client.Core.Services;
// using Cryptie.Client.Features.AddFriend.Services;
// using Cryptie.Client.Features.AddUserToGroup.ViewModels;
// using Cryptie.Client.Features.Chats.Dependencies;
// using Cryptie.Client.Features.Chats.Entities;
// using Cryptie.Client.Features.Chats.Events;
// using Cryptie.Client.Features.Chats.Services;
// using Cryptie.Client.Features.Chats.ViewModels;
// using Cryptie.Client.Features.ChatSettings.ViewModels;
// using Cryptie.Client.Features.Groups.Dependencies;
// using Cryptie.Client.Features.Groups.Services;
// using Cryptie.Client.Features.Groups.State;
// using Cryptie.Client.Features.Groups.ViewModels;
// using Cryptie.Client.Features.Menu.State;
// using Cryptie.Common.Features.Messages.DTOs;
// using Cryptie.Common.Features.UserManagement.DTOs;
// using FluentValidation;
// using Microsoft.Extensions.Options;
// using Moq;
// using ReactiveUI;
// using System.Reactive.Subjects;
// using Xunit;
//
// namespace Cryptie.Client.Tests.Features.Chats.ViewModels;
//
// public class ChatsViewModelTests
// {
//     private readonly Mock<IScreen> _screen = new();
//     private readonly Mock<IConnectionMonitor> _monitor = new();
//     private readonly Mock<IOptions<ClientOptions>> _options = new();
//     private readonly Mock<IGroupService> _groupService = new();
//     private readonly Mock<IMessagesService> _messagesService = new();
//     private readonly Mock<IGroupSelectionState> _groupState = new();
//     private readonly Mock<IUserState> _userState = new();
//     private readonly Mock<IFriendsService> _friendsService = new();
//     private readonly Mock<IValidator<AddFriendRequestDto>> _validator = new();
//     private readonly Mock<AddUserToGroupViewModel> _addUserToGroupVm = new();
//     private readonly AddFriendDependencies _addFriendDeps;
//     private readonly ChatSettingsViewModel _settingsPanel;
//     private readonly GroupsListViewModel _groupsPanel;
//     private readonly ChatsViewModelDependencies _deps;
//
//     public ChatsViewModelTests()
//     {
//         _addFriendDeps = new AddFriendDependencies(_friendsService.Object, _validator.Object, _userState.Object);
//         _settingsPanel = new ChatSettingsViewModel(_screen.Object, _groupState.Object, _options.Object, _addUserToGroupVm.Object);
//         _groupsPanel = new GroupsListViewModel(_screen.Object, _monitor.Object, _options.Object, _addFriendDeps, _groupService.Object, _messagesService.Object, _groupState.Object);
//         _deps = new ChatsViewModelDependencies(
//             _options.Object,
//             _addFriendDeps,
//             _groupService.Object,
//             _groupState.Object,
//             _messagesService.Object,
//             _settingsPanel
//         );
//     }
//
//     [Fact]
//     public void Constructor_InitializesProperties()
//     {
//         _userState.SetupGet(u => u.SessionToken).Returns(Guid.NewGuid().ToString());
//         _userState.SetupGet(u => u.UserId).Returns(Guid.NewGuid());
//         var vm = new ChatsViewModel(_screen.Object, _monitor.Object, _deps, _userState.Object);
//         Assert.NotNull(vm.GroupsPanel);
//         Assert.NotNull(vm.Messages);
//         Assert.NotNull(vm.SettingsPanel);
//         Assert.NotNull(vm.SendMessageCommand);
//         Assert.NotNull(vm.ToggleChatSettingsCommand);
//         Assert.Null(vm.MessageText);
//         Assert.False(vm.IsChatSettingsOpen);
//     }
//
//     [Fact]
//     public void MessageText_Property_Works()
//     {
//         var vm = CreateVm();
//         vm.MessageText = "abc";
//         Assert.Equal("abc", vm.MessageText);
//     }
//
//     [Fact]
//     public void IsChatSettingsOpen_Property_Works()
//     {
//         var vm = CreateVm();
//         vm.IsChatSettingsOpen = true;
//         Assert.True(vm.IsChatSettingsOpen);
//     }
//
//     [Fact]
//     public void HasGroups_And_HasNoGroups_Work()
//     {
//         var vm = CreateVm();
//         Assert.False(vm.HasGroups);
//         Assert.True(vm.HasNoGroups);
//         vm.GroupsPanel.Groups.Add(new GroupViewModel(Guid.NewGuid(), "g", false));
//         Assert.True(vm.HasGroups);
//         Assert.False(vm.HasNoGroups);
//     }
//
//     [Fact]
//     public void Dispose_DisposesDisposables()
//     {
//         var vm = CreateVm();
//         vm.Dispose();
//         // brak wyjątku = sukces
//     }
//
//     [Fact]
//     public void TryParseGuid_ParsesOrReturnsEmpty()
//     {
//         var method = typeof(ChatsViewModel).GetMethod("TryParseGuid", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
//         Assert.NotNull(method);
//         var valid = (Guid)method.Invoke(null, new object[] { Guid.NewGuid().ToString() });
//         Assert.NotEqual(Guid.Empty, valid);
//         var invalid = (Guid)method.Invoke(null, new object[] { "not-a-guid" });
//         Assert.Equal(Guid.Empty, invalid);
//     }
//
//     [Fact]
//     public void ToggleChatSettingsCommand_TogglesFlag()
//     {
//         var vm = CreateVm();
//         Assert.False(vm.IsChatSettingsOpen);
//         vm.ToggleChatSettingsCommand.Execute().Subscribe();
//         Assert.True(vm.IsChatSettingsOpen);
//         vm.ToggleChatSettingsCommand.Execute().Subscribe();
//         Assert.False(vm.IsChatSettingsOpen);
//     }
//
//     [Fact]
//     public async Task SendMessageCommand_CannotSendIfEmptyOrTooLong()
//     {
//         var vm = CreateVm();
//         vm.MessageText = null;
//         Assert.False(vm.SendMessageCommand.CanExecute.Value);
//         vm.MessageText = new string('a', 2001);
//         Assert.False(vm.SendMessageCommand.CanExecute.Value);
//         vm.MessageText = "ok";
//         _groupState.SetupGet(g => g.SelectedGroupId).Returns(Guid.NewGuid());
//         _messagesService.Setup(m => m.SendMessageToGroupViaHttpAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<string>())).Returns(Task.CompletedTask);
//         _messagesService.Setup(m => m.ConnectAsync(It.IsAny<Guid>(), It.IsAny<System.Collections.Generic.IEnumerable<Guid>>())).Returns(Task.CompletedTask);
//         _messagesService.Setup(m => m.SendMessageToGroupAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<string>())).Returns(Task.CompletedTask);
//         await vm.SendMessageCommand.Execute();
//         Assert.Equal(string.Empty, vm.MessageText);
//     }
//
//     [Fact]
//     public async Task SendMessageCommand_SwallowsException()
//     {
//         var vm = CreateVm();
//         vm.MessageText = "ok";
//         _groupState.SetupGet(g => g.SelectedGroupId).Returns(Guid.NewGuid());
//         _messagesService.Setup(m => m.SendMessageToGroupViaHttpAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<string>())).ThrowsAsync(new Exception());
//         await vm.SendMessageCommand.Execute();
//         // brak wyjątku = sukces
//     }
//
//     [Fact]
//     public void WatchGroupSelection_LoadsHistoryAndSetsMessages()
//     {
//         var gid = Guid.NewGuid();
//         _groupState.Setup(g => g.WhenAnyValue(It.IsAny<System.Linq.Expressions.Expression<Func<IGroupSelectionState, Guid>>>()))
//             .Returns(Observable.Return(gid));
//         _messagesService.Setup(m => m.ConnectAsync(It.IsAny<Guid>(), It.IsAny<System.Collections.Generic.IEnumerable<Guid>>())).Returns(Task.CompletedTask);
//         _messagesService.Setup(m => m.GetGroupMessagesAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(new[]
//         {
//             new GetGroupMessagesResponseDto.MessageDto
//             {
//                 Message = "msg",
//                 SenderId = Guid.Empty,
//                 DateTime = DateTime.UtcNow
//             }
//         });
//         var vm = CreateVm();
//         // wywołanie WatchGroupSelection następuje w konstruktorze
//         Assert.Single(vm.Messages);
//         Assert.Equal("msg", vm.Messages[0].Message);
//     }
//
//     [Fact]
//     public void WatchIncomingMessages_AddsMessageAndBumpsConversation()
//     {
//         var gid = Guid.NewGuid();
//         _groupState.SetupGet(g => g.SelectedGroupId).Returns(gid);
//         _groupState.SetupGet(g => g.SelectedGroupName).Returns("g");
//         var subject = new ReplaySubject<SignalRMessage>(1);
//         _messagesService.SetupGet(m => m.MessageReceived).Returns(subject.AsObservable());
//         var vm = CreateVm();
//         subject.OnNext(new SignalRMessage(gid, "hi", Guid.NewGuid()));
//         Assert.Single(vm.Messages);
//         Assert.Equal("hi", vm.Messages[0].Message);
//     }
//
//     private ChatsViewModel CreateVm()
//     {
//         _userState.SetupGet(u => u.SessionToken).Returns(Guid.NewGuid().ToString());
//         _userState.SetupGet(u => u.UserId).Returns(Guid.NewGuid());
//         _groupState.SetupGet(g => g.SelectedGroupName).Returns("g");
//         _groupState.SetupGet(g => g.SelectedGroupId).Returns(Guid.NewGuid());
//         _groupState.Setup(g => g.WhenAnyValue(It.IsAny<System.Linq.Expressions.Expression<Func<IGroupSelectionState, string>>>()))
//             .Returns(Observable.Return("g"));
//         _groupState.Setup(g => g.WhenAnyValue(It.IsAny<System.Linq.Expressions.Expression<Func<IGroupSelectionState, Guid>>>()))
//             .Returns(Observable.Return(Guid.NewGuid()));
//         _messagesService.SetupGet(m => m.MessageReceived).Returns(Observable.Never<SignalRMessage>());
//         return new ChatsViewModel(_screen.Object, _monitor.Object, _deps, _userState.Object);
//     }
// }
