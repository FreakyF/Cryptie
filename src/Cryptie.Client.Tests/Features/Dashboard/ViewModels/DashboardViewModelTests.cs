using Cryptie.Client.Core.Base;
using Cryptie.Client.Core.Factories;
using Cryptie.Client.Core.Navigation;
using Cryptie.Client.Features.Chats.ViewModels;
using Cryptie.Client.Features.Dashboard.ViewModels;
using Cryptie.Client.Features.Menu.ViewModels;
using Moq;
using ReactiveUI;
using Xunit;

namespace Cryptie.Client.Tests.Features.Dashboard.ViewModels;

public class DashboardViewModelTests
{
    [Fact]
    public void Constructor_InitializesPropertiesAndSetsContent()
    {
        // Arrange
        var hostScreen = new Mock<IScreen>();
        var menu = new Mock<SplitViewMenuViewModel>(hostScreen.Object);
        var vmFactory = new Mock<IViewModelFactory>();
        var chatsVm = new Mock<ChatsViewModel>(hostScreen.Object, null, null, null);
        vmFactory.Setup(f => f.Create<ChatsViewModel>(hostScreen.Object)).Returns(chatsVm.Object);

        // Act
        var vm = new DashboardViewModel(hostScreen.Object, menu.Object, vmFactory.Object);

        // Assert
        Assert.Equal(menu.Object, vm.Menu);
        Assert.NotNull(vm.Menu.ContentCoordinator);
        Assert.NotNull(vm.Content);
        Assert.Same(chatsVm.Object, vm.Content);
    }

    [Fact]
    public void Content_GetterAndSetter_Works()
    {
        var hostScreen = new Mock<IScreen>();
        var menu = new Mock<SplitViewMenuViewModel>(hostScreen.Object);
        var vmFactory = new Mock<IViewModelFactory>();
        var chatsVm = new Mock<ChatsViewModel>(hostScreen.Object, null, null, null);
        vmFactory.Setup(f => f.Create<ChatsViewModel>(hostScreen.Object)).Returns(chatsVm.Object);
        var vm = new DashboardViewModel(hostScreen.Object, menu.Object, vmFactory.Object);
        var dummy = new Mock<ViewModelBase>(hostScreen.Object).Object;
        vm.Content = dummy;
        Assert.Same(dummy, vm.Content);
    }
}

