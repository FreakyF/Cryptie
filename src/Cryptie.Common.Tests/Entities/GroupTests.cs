using System;
using System.Collections.Generic;
using Cryptie.Common.Entities;
using Xunit;

namespace Cryptie.Common.Tests.Entities;

public class GroupTests
{
    [Fact]
    public void Can_Create_Group_With_Valid_Properties()
    {
        // Arrange
        var id = Guid.NewGuid();
        var name = "TestGroup";
        var isPrivate = true;
        var members = new HashSet<User> { new User { Id = Guid.NewGuid() } };
        var messages = new HashSet<GroupMessage> { new GroupMessage { Id = Guid.NewGuid() } };
        var keys = new HashSet<GroupEncryptionKey> { new GroupEncryptionKey { Id = Guid.NewGuid() } };

        // Act
        var group = new Group
        {
            Id = id,
            Name = name,
            IsPrivate = isPrivate,
            Members = members,
            Messages = messages,
            GroupEncryptionKeys = keys
        };

        // Assert
        Assert.Equal(id, group.Id);
        Assert.Equal(name, group.Name);
        Assert.Equal(isPrivate, group.IsPrivate);
        Assert.Equal(members, group.Members);
        Assert.Equal(messages, group.Messages);
        Assert.Equal(keys, group.GroupEncryptionKeys);
    }

    [Fact]
    public void Name_Should_Not_Be_Null_By_Default()
    {
        // Arrange
        var group = new Group();
        // Assert
        Assert.NotNull(group.Name);
    }

    [Fact]
    public void Members_Should_Not_Be_Null_By_Default()
    {
        var group = new Group();
        Assert.NotNull(group.Members);
    }

    [Fact]
    public void Messages_Should_Not_Be_Null_By_Default()
    {
        var group = new Group();
        Assert.NotNull(group.Messages);
    }

    [Fact]
    public void GroupEncryptionKeys_Should_Not_Be_Null_By_Default()
    {
        var group = new Group();
        Assert.NotNull(group.GroupEncryptionKeys);
    }

    [Fact]
    public void Can_Set_And_Get_Properties()
    {
        var group = new Group();
        var id = Guid.NewGuid();
        var name = "AnotherGroup";
        var isPrivate = false;
        var members = new List<User> { new User { Id = Guid.NewGuid() } };
        var messages = new List<GroupMessage> { new GroupMessage { Id = Guid.NewGuid() } };
        var keys = new List<GroupEncryptionKey> { new GroupEncryptionKey { Id = Guid.NewGuid() } };

        group.Id = id;
        group.Name = name;
        group.IsPrivate = isPrivate;
        group.Members = members;
        group.Messages = messages;
        group.GroupEncryptionKeys = keys;

        Assert.Equal(id, group.Id);
        Assert.Equal(name, group.Name);
        Assert.Equal(isPrivate, group.IsPrivate);
        Assert.Equal(members, group.Members);
        Assert.Equal(messages, group.Messages);
        Assert.Equal(keys, group.GroupEncryptionKeys);
    }
}
