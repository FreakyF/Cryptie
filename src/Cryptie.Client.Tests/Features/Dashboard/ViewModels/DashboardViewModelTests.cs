// using Cryptie.Client.Core.Base;
// using Cryptie.Client.Core.Factories;
// using Cryptie.Client.Core.Navigation;
// using Cryptie.Client.Core.Services;
// using Cryptie.Client.Features.Chats.Dependencies;
// using Cryptie.Client.Features.Chats.ViewModels;
// using Cryptie.Client.Features.Dashboard.ViewModels;
// using Cryptie.Client.Features.Menu.State;
// using Cryptie.Client.Features.Menu.ViewModels;
// using Moq;
// using ReactiveUI;
// using Xunit;
//
// namespace Cryptie.Client.Tests.Features.Dashboard.ViewModels;
//
// public class DashboardViewModelTests
// {
//     [Fact]
//     public void Constructor_InitializesPropertiesAndSetsContent()
//     {
//         // Arrange
//         var hostScreen = new Mock<IScreen>();
//         var userDetails = new Mock<IUserDetailsService>();
//         var connectionMonitor = new Mock<IConnectionMonitor>();
//         connectionMonitor.SetupGet(m => m.ConnectionStatusChanged).Returns(System.Reactive.Linq.Observable.Return(true));
//         var userState = new Mock<IUserState>();
//         var menu = new SplitViewMenuViewModel(userDetails.Object, connectionMonitor.Object, userState.Object);
//         var vmFactory = new Mock<IViewModelFactory>();
//         var options = new Mock<Microsoft.Extensions.Options.IOptions<Cryptie.Client.Configuration.ClientOptions>>();
//         var friendsService = new Mock<Cryptie.Client.Features.AddFriend.Services.IFriendsService>();
//         var validator = new Mock<FluentValidation.IValidator<Cryptie.Common.Features.UserManagement.DTOs.AddFriendRequestDto>>();
//         var addFriendDeps = new Cryptie.Client.Features.Groups.Dependencies.AddFriendDependencies(friendsService.Object, validator.Object, userState.Object);
//         var groupService = new Mock<Cryptie.Client.Features.Groups.Services.IGroupService>();
//         var groupState = new Mock<Cryptie.Client.Features.Groups.State.IGroupSelectionState>();
//         var messagesService = new Mock<Cryptie.Client.Features.Chats.Services.IMessagesService>();
//         var addUserToGroupVm = new Cryptie.Client.Features.AddUserToGroup.ViewModels.AddUserToGroupViewModel(hostScreen.Object);
//         var settingsPanel = new Cryptie.Client.Features.ChatSettings.ViewModels.ChatSettingsViewModel(
//             hostScreen.Object,
//             groupState.Object,
//             options.Object,
//             userState.Object
//         );
//         var chatsDeps = new Cryptie.Client.Features.Chats.Dependencies.ChatsViewModelDependencies(
//             options.Object,
//             addFriendDeps,
//             groupService.Object,
//             groupState.Object,
//             messagesService.Object,
//             settingsPanel
//         );
//         var chatsVm = new ChatsViewModel(hostScreen.Object, connectionMonitor.Object, chatsDeps, userState.Object);
//         vmFactory.Setup(f => f.Create<ChatsViewModel>(hostScreen.Object)).Returns(chatsVm);
//
//         // Act
//         var vm = new DashboardViewModel(hostScreen.Object, menu, vmFactory.Object);
//
//         // Assert
//         Assert.Equal(menu, vm.Menu);
//         Assert.NotNull(vm.Menu.ContentCoordinator);
//         Assert.NotNull(vm.Content);
//         Assert.Same(chatsVm, vm.Content);
//     }
//
//     [Fact]
//     public void Content_GetterAndSetter_Works()
//     {
//         var hostScreen = new Mock<IScreen>();
//         var userDetails = new Mock<IUserDetailsService>();
//         var connectionMonitor = new Mock<IConnectionMonitor>();
//         connectionMonitor.SetupGet(m => m.ConnectionStatusChanged).Returns(System.Reactive.Linq.Observable.Return(true));
//         var userState = new Mock<IUserState>();
//         var menu = new SplitViewMenuViewModel(userDetails.Object, connectionMonitor.Object, userState.Object);
//         var vmFactory = new Mock<IViewModelFactory>();
//         var options = new Mock<Microsoft.Extensions.Options.IOptions<Cryptie.Client.Configuration.ClientOptions>>();
//         var friendsService = new Mock<Cryptie.Client.Features.AddFriend.Services.IFriendsService>();
//         var validator = new Mock<FluentValidation.IValidator<Cryptie.Common.Features.UserManagement.DTOs.AddFriendRequestDto>>();
//         var addFriendDeps = new Cryptie.Client.Features.Groups.Dependencies.AddFriendDependencies(friendsService.Object, validator.Object, userState.Object);
//         var groupService = new Mock<Cryptie.Client.Features.Groups.Services.IGroupService>();
//         var groupState = new Mock<Cryptie.Client.Features.Groups.State.IGroupSelectionState>();
//         var messagesService = new Mock<Cryptie.Client.Features.Chats.Services.IMessagesService>();
//         var addUserToGroupVm = new Cryptie.Client.Features.AddUserToGroup.ViewModels.AddUserToGroupViewModel(hostScreen.Object);
//         var settingsPanel = new Cryptie.Client.Features.ChatSettings.ViewModels.ChatSettingsViewModel(
//             hostScreen.Object,
//             groupState.Object,
//             options.Object,
//             userState.Object
//         );
//         var chatsDeps = new Cryptie.Client.Features.Chats.Dependencies.ChatsViewModelDependencies(
//             options.Object,
//             addFriendDeps,
//             groupService.Object,
//             groupState.Object,
//             messagesService.Object,
//             settingsPanel
//         );
//         var chatsVm = new ChatsViewModel(hostScreen.Object, connectionMonitor.Object, chatsDeps, userState.Object);
//         vmFactory.Setup(f => f.Create<ChatsViewModel>(hostScreen.Object)).Returns(chatsVm);
//         var vm = new DashboardViewModel(hostScreen.Object, menu, vmFactory.Object);
//         var dummy = new Mock<ViewModelBase>(hostScreen.Object).Object;
//         vm.Content = dummy;
//         Assert.Same(dummy, vm.Content);
//     }
// }
