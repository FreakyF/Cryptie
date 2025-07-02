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
    [InlineData("msg1", false, "G1")]
    [InlineData("", true, "")] // test pustych wartości
    [InlineData(null, false, null)] // test nulli
    public void Properties_AreSetCorrectly(string? message, bool isOwn, string? groupName)
    {
        var vm = new ChatMessageViewModel(message, isOwn, groupName);
        Assert.Equal(message, vm.Message);
        Assert.Equal(isOwn, vm.IsOwn);
        Assert.Equal(groupName, vm.GroupName);
    }

    [Fact]
    public void ChatMessageViewModel_IsSealed()
    {
        Assert.True(typeof(ChatMessageViewModel).IsSealed);
    }
}
