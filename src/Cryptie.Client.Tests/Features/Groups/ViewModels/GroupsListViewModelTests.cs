using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Cryptie.Client.Configuration;
using Cryptie.Client.Core.Services;
using Cryptie.Client.Encryption;
using Cryptie.Client.Features.AddFriend.Services;
using Cryptie.Client.Features.AddFriend.ViewModels;
using Cryptie.Client.Features.Chats.Events;
using Cryptie.Client.Features.Chats.Services;
using Cryptie.Client.Features.Groups.Dependencies;
using Cryptie.Client.Features.Groups.Services;
using Cryptie.Client.Features.Groups.State;
using Cryptie.Client.Features.Groups.ViewModels;
using Cryptie.Client.Features.Menu.State;
using Cryptie.Common.Features.GroupManagement;
using Cryptie.Common.Features.GroupManagement.DTOs;
using Cryptie.Common.Features.KeysManagement.DTOs;
using Cryptie.Common.Features.Messages.DTOs;
using Cryptie.Common.Features.UserManagement.DTOs;
using DynamicData.Kernel;
using FluentValidation;
using Microsoft.Extensions.Options;
using Moq;
using ReactiveUI;
using Xunit;

namespace Cryptie.Client.Tests.Features.Groups.ViewModels;

public class GroupsListViewModelTests
{
    private readonly Mock<IGroupService> _groupService = new();
    private readonly Mock<IMessagesService> _messagesService = new();
    private readonly Mock<IUserState> _userState = new();
    private readonly Mock<IGroupSelectionState> _groupState = new();
    private readonly Mock<IKeyService> _keyService = new();
    private readonly Mock<IConnectionMonitor> _connectionMonitor = new();
    private readonly Mock<IScreen> _hostScreen = new();
    private readonly Mock<IFriendsService> _friendsService = new();
    private readonly Mock<IValidator<AddFriendRequestDto>> _validator = new();
    private readonly Mock<IUserDetailsService> _userDetailsService = new();
    private readonly IOptions<ClientOptions> _options = Options.Create(new ClientOptions { FontUri = "iconUri" });

    private GroupsListViewModel CreateVm()
    {
        // Dodano mockowanie ConnectionStatusChanged, aby uniknąć ArgumentNullException
        _connectionMonitor.Setup(x => x.ConnectionStatusChanged).Returns(System.Reactive.Linq.Observable.Return(true));
        var deps = new AddFriendDependencies(_friendsService.Object, _validator.Object, _userState.Object, _userDetailsService.Object, _keyService.Object);
        return new GroupsListViewModel(
            _hostScreen.Object,
            _connectionMonitor.Object,
            _options,
            deps,
            _groupService.Object,
            _messagesService.Object,
            _groupState.Object,
            _keyService.Object
        );
    }

    [Fact]
    public void Constructor_InitializesPropertiesAndSubscribes()
    {
        var vm = CreateVm();
        Assert.NotNull(vm.Groups);
        Assert.Null(vm.SelectedGroup);
        Assert.Equal("iconUri", vm.IconUri);
        Assert.NotNull(vm.AddFriendCommand);
        Assert.NotNull(vm.ShowAddFriend);
        Assert.NotNull(vm.GroupKeyCache);
    }

    [Fact]
    public async Task AddFriendCommand_ExecutesAndLoadsGroups()
    {
        var vm = CreateVm();
        bool showCalled = false;
        vm.ShowAddFriend.RegisterHandler(ctx =>
        {
            showCalled = true;
            ctx.SetOutput(Unit.Default);
        });
        _groupService.Setup(x => x.GetGroupsNamesAsync(It.IsAny<GetGroupsNamesRequestDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dictionary<Guid, string>());
        _groupService.Setup(x => x.GetGroupsPrivacyAsync(It.IsAny<IsGroupsPrivateRequestDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dictionary<Guid, bool>());
        _keyService.Setup(x => x.GetGroupsKeyAsync(It.IsAny<GetGroupsKeyRequestDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetGroupsKeyResponseDto { Keys = new Dictionary<Guid, string>() });
        _userState.SetupGet(x => x.SessionToken).Returns(Guid.NewGuid().ToString());
        _userState.SetupGet(x => x.PrivateKey).Returns("priv");
        _messagesService.Setup(x => x.GetGroupMessagesAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
            .ReturnsAsync(new List<GetGroupMessagesResponseDto.MessageDto>());
        await vm.AddFriendCommand.Execute().ToTask();
        Assert.True(showCalled);
    }

    [Fact]
    public void AddFriendCommand_ThrownExceptions_AreHandled()
    {
        var vm = CreateVm();
        Exception? thrown = null;
        vm.AddFriendCommand.ThrownExceptions.Subscribe(ex => thrown = ex);
        // Simulate exception in ShowAddFriend
        vm.ShowAddFriend.RegisterHandler(ctx => throw new Exception("fail"));
        var task = vm.AddFriendCommand.Execute().ToTask();
        Assert.ThrowsAsync<Exception>(() => task);
    }

    [Fact]
    public void OnConversationBumped_MovesGroupToTopAndUpdatesSelectedGroup()
    {
        var vm = CreateVm();
        var groupId = Guid.NewGuid();
        var groupId2 = Guid.NewGuid();
        var groupName = "Group1";
        var groupName2 = "Group2";
        // Ustaw pola prywatne przez refleksję
        typeof(GroupsListViewModel).GetField("_groupIds", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!.SetValue(vm, new List<Guid> { groupId, groupId2 });
        vm.Groups.Add(groupName);
        vm.Groups.Add(groupName2);
        _groupState.SetupGet(x => x.SelectedGroupId).Returns(groupId2);
        var evt = new ConversationBumped(groupId2, DateTime.UtcNow);
        vm.SelectedGroup = groupName2;
        vm.GetType().GetMethod("OnConversationBumped", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
            .Invoke(vm, new object[] { evt });
        Assert.Equal(groupName2, vm.Groups[0]);
        Assert.Equal(groupId2, ((List<Guid>)typeof(GroupsListViewModel).GetField("_groupIds", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!.GetValue(vm)!)[0]);
    }

    [Fact]
    public async Task LoadGroupsAsync_HandlesAllBranches()
    {
        var vm = CreateVm();
        var method = vm.GetType().GetMethod("LoadGroupsAsync", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!;
        // Brak tokena
        _userState.SetupGet(x => x.SessionToken).Returns((string?)null);
        var t1 = (Task)method.Invoke(vm, new object[] { CancellationToken.None })!;
        await t1;
        // Token nie jest guidem
        _userState.SetupGet(x => x.SessionToken).Returns("not-a-guid");
        var t2 = (Task)method.Invoke(vm, new object[] { CancellationToken.None })!;
        await t2;
        // Token ok, brak kluczy
        var guid = Guid.NewGuid();
        _userState.SetupGet(x => x.SessionToken).Returns(guid.ToString());
        _userState.SetupGet(x => x.PrivateKey).Returns("priv");
        _groupService.Setup(x => x.GetGroupsNamesAsync(It.IsAny<GetGroupsNamesRequestDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dictionary<Guid, string> { { guid, "g1" } });
        _groupService.Setup(x => x.GetGroupsPrivacyAsync(It.IsAny<IsGroupsPrivateRequestDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dictionary<Guid, bool> { { guid, true } });
        _keyService.Setup(x => x.GetGroupsKeyAsync(It.IsAny<GetGroupsKeyRequestDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetGroupsKeyResponseDto { Keys = null });
        _messagesService.Setup(x => x.GetGroupMessagesAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
            .ReturnsAsync(new List<GetGroupMessagesResponseDto.MessageDto>());
        var t3 = (Task)method.Invoke(vm, new object[] { CancellationToken.None })!;
        await t3;
        // Wyjątek in kluczach
        _keyService.Setup(x => x.GetGroupsKeyAsync(It.IsAny<GetGroupsKeyRequestDto>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("fail"));
        await Assert.ThrowsAsync<Exception>(async () =>
        {
            var t4 = (Task)method.Invoke(vm, new object[] { CancellationToken.None })!;
            await t4;
        });
    }

    [Fact]
    public void SelectedGroup_Property_Works()
    {
        var vm = CreateVm();
        vm.SelectedGroup = "abc";
        Assert.Equal("abc", vm.SelectedGroup);
    }

    [Fact]
    public void GroupKeyCache_Property_Works()
    {
        var vm = CreateVm();
        Assert.NotNull(vm.GroupKeyCache);
    }

    [Fact]
    public void Groups_Property_Works()
    {
        var vm = CreateVm();
        Assert.NotNull(vm.Groups);
    }

    [Fact]
    public void IconUri_Property_Works()
    {
        var vm = CreateVm();
        Assert.Equal("iconUri", vm.IconUri);
    }
}
