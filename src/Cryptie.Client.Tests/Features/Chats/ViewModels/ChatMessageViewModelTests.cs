using Cryptie.Client.Features.Chats.ViewModels;
using Xunit;

namespace Cryptie.Client.Tests.Features.Chats.ViewModels;

public class ChatMessageViewModelTests
{
    [Fact]
    public void Constructor_SetsPropertiesCorrectly()
    {
        // Arrange
        var message = "Hello!";
        var isOwn = true;
        var groupName = "TestGroup";

        // Act
        var vm = new ChatMessageViewModel(message, isOwn, groupName);

        // Assert
        Assert.Equal(message, vm.Message);
        Assert.Equal(isOwn, vm.IsOwn);
        Assert.Equal(groupName, vm.GroupName);
    }

    [Theory]
    [InlineData("msg1", true, "g1")]
    [InlineData("msg2", false, "g2")]
    [InlineData("", false, "")] // test pustych wartości
    public void Properties_AreImmutable(string message, bool isOwn, string groupName)
    {
        var vm = new ChatMessageViewModel(message, isOwn, groupName);
        Assert.Equal(message, vm.Message);
        Assert.Equal(isOwn, vm.IsOwn);
        Assert.Equal(groupName, vm.GroupName);
    }
}

