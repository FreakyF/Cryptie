using System;
using System.Collections.Generic;
using System.Linq;
using Cryptie.Common.Entities;
using Cryptie.Server.Persistence.DatabaseContext;
using Cryptie.Server.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Xunit;

public class DatabaseServiceTests
{
    private readonly Mock<IAppDbContext> _dbContextMock = new();
    private readonly DatabaseService _service;

    public DatabaseServiceTests()
    {
        _service = new DatabaseService(_dbContextMock.Object);
    }

    [Fact]
    public void GetUserFromToken_ReturnsUser_WhenTokenExists()
    {
        var user = new User { Id = Guid.NewGuid() };
        var token = new UserToken { Id = Guid.NewGuid(), User = user };
        var tokens = new List<UserToken> { token }.AsQueryable();
        var dbSetMock = tokens.BuildMockDbSet();
        _dbContextMock.SetupGet(x => x.UserTokens).Returns(dbSetMock.Object);

        var result = _service.GetUserFromToken(token.Id);
        Assert.Equal(user, result);
    }

    [Fact]
    public void GetUserFromToken_ReturnsNull_WhenTokenNotExists()
    {
        var tokens = new List<UserToken>().AsQueryable();
        var dbSetMock = tokens.BuildMockDbSet();
        _dbContextMock.SetupGet(x => x.UserTokens).Returns(dbSetMock.Object);

        var result = _service.GetUserFromToken(Guid.NewGuid());
        Assert.Null(result);
    }

    [Fact]
    public void FindUserById_ReturnsUser_WhenExists()
    {
        var user = new User { Id = Guid.NewGuid() };
        var dbSetMock = new Mock<DbSet<User>>();
        dbSetMock.Setup(x => x.Find(It.IsAny<Guid>())).Returns(user);
        _dbContextMock.SetupGet(x => x.Users).Returns(dbSetMock.Object);

        var result = _service.FindUserById(user.Id);
        Assert.Equal(user, result);
    }

    [Fact]
    public void FindUserById_ReturnsNull_WhenNotExists()
    {
        var dbSetMock = new Mock<DbSet<User>>();
        dbSetMock.Setup(x => x.Find(It.IsAny<Guid>())).Returns((User)null!);
        _dbContextMock.SetupGet(x => x.Users).Returns(dbSetMock.Object);

        var result = _service.FindUserById(Guid.NewGuid());
        Assert.Null(result);
    }

    [Fact]
    public void FindUserByLogin_ReturnsUser_WhenExists()
    {
        var user = new User { Login = "test" };
        var users = new List<User> { user }.AsQueryable();
        var dbSetMock = users.BuildMockDbSet();
        _dbContextMock.SetupGet(x => x.Users).Returns(dbSetMock.Object);

        var result = _service.FindUserByLogin("test");
        Assert.Equal(user, result);
    }

    [Fact]
    public void FindUserByLogin_ReturnsNull_WhenNotExists()
    {
        var users = new List<User>().AsQueryable();
        var dbSetMock = users.BuildMockDbSet();
        _dbContextMock.SetupGet(x => x.Users).Returns(dbSetMock.Object);

        var result = _service.FindUserByLogin("notfound");
        Assert.Null(result);
    }

    [Fact]
    public void FindGroupById_ReturnsGroup_WhenExists()
    {
        var group = new Group { Id = Guid.NewGuid() };
        var groups = new List<Group> { group }.AsQueryable();
        var dbSetMock = groups.BuildMockDbSet();
        _dbContextMock.SetupGet(x => x.Groups).Returns(dbSetMock.Object);

        var result = _service.FindGroupById(group.Id);
        Assert.Equal(group, result);
    }

    [Fact]
    public void FindGroupById_ReturnsNull_WhenNotExists()
    {
        var groups = new List<Group>().AsQueryable();
        var dbSetMock = groups.BuildMockDbSet();
        _dbContextMock.SetupGet(x => x.Groups).Returns(dbSetMock.Object);

        var result = _service.FindGroupById(Guid.NewGuid());
        Assert.Null(result);
    }

    [Fact]
    public void CreateNewGroup_AddsGroupAndSavesChanges()
    {
        var user = new User();
        var group = new Group { Name = "g", Members = { user } };
        var dbSetMock = new Mock<DbSet<Group>>();
        var entityEntry = CreateEntityEntry(group);
        dbSetMock.Setup(x => x.Add(It.IsAny<Group>())).Returns(entityEntry);
        _dbContextMock.SetupGet(x => x.Groups).Returns(dbSetMock.Object);
        _dbContextMock.Setup(x => x.SaveChanges()).Verifiable();

        _service.CreateNewGroup(user, "g");
        _dbContextMock.Verify(x => x.SaveChanges(), Times.Once);
    }

    [Fact]
    public void AddUserToGroup_AddsUserAndSavesChanges_WhenUserAndGroupExist()
    {
        var userId = Guid.NewGuid();
        var groupId = Guid.NewGuid();
        var user = new User { Id = userId };
        var group = new Group { Id = groupId };
        var users = new List<User> { user }.AsQueryable();
        var groups = new List<Group> { group }.AsQueryable();
        var usersDbSet = users.BuildMockDbSet();
        var groupsDbSet = groups.BuildMockDbSet();
        _dbContextMock.SetupGet(x => x.Users).Returns(usersDbSet.Object);
        _dbContextMock.SetupGet(x => x.Groups).Returns(groupsDbSet.Object);
        _dbContextMock.Setup(x => x.SaveChanges()).Verifiable();

        _service.AddUserToGroup(userId, groupId);
        Assert.Contains(user, group.Members);
        _dbContextMock.Verify(x => x.SaveChanges(), Times.Once);
    }

    [Fact]
    public void AddUserToGroup_DoesNothing_WhenUserNotFound()
    {
        var userId = Guid.NewGuid();
        var groupId = Guid.NewGuid();
        var users = new List<User>().AsQueryable();
        var groups = new List<Group> { new Group { Id = groupId } }.AsQueryable();
        var usersDbSet = users.BuildMockDbSet();
        var groupsDbSet = groups.BuildMockDbSet();
        _dbContextMock.SetupGet(x => x.Users).Returns(usersDbSet.Object);
        _dbContextMock.SetupGet(x => x.Groups).Returns(groupsDbSet.Object);

        _service.AddUserToGroup(userId, groupId);
        _dbContextMock.Verify(x => x.SaveChanges(), Times.Never);
    }

    [Fact]
    public void RemoveUserFromGroup_RemovesUserAndSavesChanges_WhenUserAndGroupExist()
    {
        var userId = Guid.NewGuid();
        var groupId = Guid.NewGuid();
        var user = new User { Id = userId };
        var group = new Group { Id = groupId, Members = { user } };
        var users = new List<User> { user }.AsQueryable();
        var groups = new List<Group> { group }.AsQueryable();
        var usersDbSet = users.BuildMockDbSet();
        var groupsDbSet = groups.BuildMockDbSet();
        _dbContextMock.SetupGet(x => x.Users).Returns(usersDbSet.Object);
        _dbContextMock.SetupGet(x => x.Groups).Returns(groupsDbSet.Object);
        _dbContextMock.Setup(x => x.SaveChanges()).Verifiable();

        _service.RemoveUserFromGroup(userId, groupId);
        Assert.DoesNotContain(user, group.Members);
        _dbContextMock.Verify(x => x.SaveChanges(), Times.Once);
    }

    [Fact]
    public void RemoveUserFromGroup_DoesNothing_WhenUserNotFound()
    {
        var userId = Guid.NewGuid();
        var groupId = Guid.NewGuid();
        var users = new List<User>().AsQueryable();
        var groups = new List<Group> { new Group { Id = groupId } }.AsQueryable();
        var usersDbSet = users.BuildMockDbSet();
        var groupsDbSet = groups.BuildMockDbSet();
        _dbContextMock.SetupGet(x => x.Users).Returns(usersDbSet.Object);
        _dbContextMock.SetupGet(x => x.Groups).Returns(groupsDbSet.Object);

        _service.RemoveUserFromGroup(userId, groupId);
        _dbContextMock.Verify(x => x.SaveChanges(), Times.Never);
    }

    [Fact]
    public void DeleteGroup_RemovesGroupAndSavesChanges_WhenGroupExists()
    {
        var groupId = Guid.NewGuid();
        var group = new Group { Id = groupId };
        var groups = new List<Group> { group }.AsQueryable();
        var groupsDbSet = groups.BuildMockDbSet();
        groupsDbSet.Setup(x => x.Remove(It.IsAny<Group>())).Verifiable();
        _dbContextMock.SetupGet(x => x.Groups).Returns(groupsDbSet.Object);
        _dbContextMock.Setup(x => x.SaveChanges()).Verifiable();

        var result = _service.DeleteGroup(groupId);
        Assert.True(result);
        groupsDbSet.Verify(x => x.Remove(group), Times.Once);
        _dbContextMock.Verify(x => x.SaveChanges(), Times.Once);
    }

    [Fact]
    public void DeleteGroup_ReturnsFalse_WhenGroupNotFound()
    {
        var groupId = Guid.NewGuid();
        var groups = new List<Group>().AsQueryable();
        var groupsDbSet = groups.BuildMockDbSet();
        _dbContextMock.SetupGet(x => x.Groups).Returns(groupsDbSet.Object);

        var result = _service.DeleteGroup(groupId);
        Assert.False(result);
    }

    [Fact]
    public void ChangeGroupName_ChangesNameAndSaves_WhenGroupExists()
    {
        var groupId = Guid.NewGuid();
        var group = new Group { Id = groupId, Name = "old" };
        var groups = new List<Group> { group }.AsQueryable();
        var groupsDbSet = groups.BuildMockDbSet();
        _dbContextMock.SetupGet(x => x.Groups).Returns(groupsDbSet.Object);
        _dbContextMock.Setup(x => x.SaveChanges()).Verifiable();

        var result = _service.ChangeGroupName(groupId, "new");
        Assert.True(result);
        Assert.Equal("new", group.Name);
        _dbContextMock.Verify(x => x.SaveChanges(), Times.Once);
    }

    [Fact]
    public void ChangeGroupName_ReturnsFalse_WhenGroupNotFound()
    {
        var groupId = Guid.NewGuid();
        var groups = new List<Group>().AsQueryable();
        var groupsDbSet = groups.BuildMockDbSet();
        _dbContextMock.SetupGet(x => x.Groups).Returns(groupsDbSet.Object);

        var result = _service.ChangeGroupName(groupId, "new");
        Assert.False(result);
    }

    [Fact]
    public void CreateTotpToken_AddsTokenAndSavesChanges()
    {
        var user = new User();
        var dbSetMock = new Mock<DbSet<TotpToken>>();
        var token = new TotpToken { Id = Guid.NewGuid(), User = user, Until = DateTime.UtcNow.AddMinutes(5) };
        var entityEntry = CreateEntityEntry(token);
        dbSetMock.Setup(x => x.Add(It.IsAny<TotpToken>())).Returns(entityEntry);
        _dbContextMock.SetupGet(x => x.TotpTokens).Returns(dbSetMock.Object);
        _dbContextMock.Setup(x => x.SaveChanges()).Verifiable();

        var result = _service.CreateTotpToken(user);
        Assert.Equal(token.Id, result);
        _dbContextMock.Verify(x => x.SaveChanges(), Times.Once);
    }

    [Fact]
    public void LogLoginAttempt_AddsUserLoginAttemptAndSavesChanges()
    {
        var user = new User();
        var dbSetMock = new Mock<DbSet<UserLoginAttempt>>();
        dbSetMock.Setup(x => x.Add(It.IsAny<UserLoginAttempt>())).Verifiable();
        _dbContextMock.SetupGet(x => x.UserLoginAttempts).Returns(dbSetMock.Object);
        _dbContextMock.Setup(x => x.SaveChanges()).Verifiable();

        _service.LogLoginAttempt(user);
        dbSetMock.Verify(x => x.Add(It.IsAny<UserLoginAttempt>()), Times.Once);
        _dbContextMock.Verify(x => x.SaveChanges(), Times.Once);
    }

    [Fact]
    public void LogLoginAttempt_AddsHoneypotLoginAttemptAndSavesChanges()
    {
        var dbSetMock = new Mock<DbSet<HoneypotLoginAttempt>>();
        dbSetMock.Setup(x => x.Add(It.IsAny<HoneypotLoginAttempt>())).Verifiable();
        _dbContextMock.SetupGet(x => x.HoneypotLoginAttempts).Returns(dbSetMock.Object);
        _dbContextMock.Setup(x => x.SaveChanges()).Verifiable();

        _service.LogLoginAttempt("honeypot");
        dbSetMock.Verify(x => x.Add(It.IsAny<HoneypotLoginAttempt>()), Times.Once);
        _dbContextMock.Verify(x => x.SaveChanges(), Times.Once);
    }

    [Fact]
    public void GenerateUserToken_AddsTokenAndSavesChanges()
    {
        var user = new User();
        var dbSetMock = new Mock<DbSet<UserToken>>();
        var token = new UserToken { Id = Guid.NewGuid(), User = user };
        var entityEntry = CreateEntityEntry(token);
        dbSetMock.Setup(x => x.Add(It.IsAny<UserToken>())).Returns(entityEntry);
        _dbContextMock.SetupGet(x => x.UserTokens).Returns(dbSetMock.Object);
        _dbContextMock.Setup(x => x.SaveChanges()).Verifiable();

        var result = _service.GenerateUserToken(user);
        Assert.Equal(token.Id, result);
        _dbContextMock.Verify(x => x.SaveChanges(), Times.Once);
    }

    [Fact]
    public void AddFriend_AddsFriendAndSavesChanges()
    {
        var user = new User();
        var friend = new User();
        _dbContextMock.Setup(x => x.SaveChanges()).Verifiable();

        _service.AddFriend(user, friend);
        Assert.Contains(friend, user.Friends);
        _dbContextMock.Verify(x => x.SaveChanges(), Times.Once);
    }

    [Fact]
    public void CreateGroup_AddsGroupAndSavesChanges()
    {
        var dbSetMock = new Mock<DbSet<Group>>();
        var group = new Group { Name = "g", IsPrivate = true };
        var entityEntry = CreateEntityEntry(group);
        dbSetMock.Setup(x => x.Add(It.IsAny<Group>())).Returns(entityEntry);
        _dbContextMock.SetupGet(x => x.Groups).Returns(dbSetMock.Object);
        _dbContextMock.Setup(x => x.SaveChanges()).Verifiable();

        var result = _service.CreateGroup("g", true);
        Assert.Equal("g", result.Name);
        Assert.True(result.IsPrivate);
        _dbContextMock.Verify(x => x.SaveChanges(), Times.Once);
    }

    [Fact]
    public void ChangeUserDisplayName_ChangesNameAndSavesChanges()
    {
        var user = new User();
        _dbContextMock.Setup(x => x.SaveChanges()).Verifiable();

        _service.ChangeUserDisplayName(user, "newName");
        Assert.Equal("newName", user.DisplayName);
        _dbContextMock.Verify(x => x.SaveChanges(), Times.Once);
    }

    [Fact]
    public void GetUserPublicKey_ReturnsPublicKey_WhenUserExists()
    {
        var userId = Guid.NewGuid();
        var user = new User { Id = userId, PublicKey = "pub" };
        var users = new List<User> { user }.AsQueryable();
        var dbSetMock = users.BuildMockDbSet();
        _dbContextMock.SetupGet(x => x.Users).Returns(dbSetMock.Object);

        var result = _service.GetUserPublicKey(userId);
        Assert.Equal("pub", result);
    }

    [Fact]
    public void GetUserPublicKey_ReturnsEmpty_WhenUserNotExists()
    {
        var users = new List<User>().AsQueryable();
        var dbSetMock = users.BuildMockDbSet();
        _dbContextMock.SetupGet(x => x.Users).Returns(dbSetMock.Object);

        var result = _service.GetUserPublicKey(Guid.NewGuid());
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void SaveUserKeys_SetsKeysAndSavesChanges()
    {
        var user = new User();
        _dbContextMock.Setup(x => x.SaveChanges()).Verifiable();

        _service.SaveUserKeys(user, "priv", "pub");
        Assert.Equal("priv", user.PrivateKey);
        Assert.Equal("pub", user.PublicKey);
        _dbContextMock.Verify(x => x.SaveChanges(), Times.Once);
    }

    [Fact]
    public void SendGroupMessage_AddsMessageAndSavesChanges()
    {
        var group = new Group();
        var sender = new User();
        var dbSetMock = new Mock<DbSet<GroupMessage>>();
        var message = new GroupMessage
            { Id = Guid.NewGuid(), Group = group, Sender = sender, Message = "msg", DateTime = DateTime.UtcNow };
        var entityEntry = CreateEntityEntry(message);
        dbSetMock.Setup(x => x.Add(It.IsAny<GroupMessage>())).Returns(entityEntry);
        _dbContextMock.SetupGet(x => x.GroupMessages).Returns(dbSetMock.Object);
        _dbContextMock.Setup(x => x.SaveChanges()).Verifiable();

        var result = _service.SendGroupMessage(group, sender, "msg");
        Assert.Equal("msg", result.Message);
        Assert.Equal(group, result.Group);
        Assert.Equal(sender, result.Sender);
        _dbContextMock.Verify(x => x.SaveChanges(), Times.Once);
    }

    [Fact]
    public void GetGroupMessage_ReturnsMessage_WhenExists()
    {
        var groupId = Guid.NewGuid();
        var messageId = Guid.NewGuid();
        var group = new Group
        {
            Id = groupId,
            GroupEncryptionKeys = new List<GroupEncryptionKey>(),
            IsPrivate = false,
            Members = new List<User>(),
            Messages = new List<GroupMessage>()
        };
        var message = new GroupMessage
        {
            Id = messageId,
            Group = group,
            GroupId = groupId,
            DateTime = default,
            Message = string.Empty,
            Sender = null
        };
        var messages = new List<GroupMessage> { message }.AsQueryable();
        var dbSetMock = messages.BuildMockDbSet();
        _dbContextMock.SetupGet(x => x.GroupMessages).Returns(dbSetMock.Object);

        var result = _service.GetGroupMessage(messageId, groupId);
        Assert.Equal(message, result);
    }

    [Fact]
    public void GetGroupMessage_ReturnsNull_WhenNotExists()
    {
        var groupId = Guid.NewGuid();
        var messageId = Guid.NewGuid();
        var messages = new List<GroupMessage>().AsQueryable();
        var dbSetMock = messages.BuildMockDbSet();
        _dbContextMock.SetupGet(x => x.GroupMessages).Returns(dbSetMock.Object);

        var result = _service.GetGroupMessage(messageId, groupId);
        Assert.Null(result);
    }

    [Fact]
    public void GetGroupMessages_ReturnsMessagesForGroup()
    {
        var groupId = Guid.NewGuid();
        var group = new Group { Id = groupId };
        var message1 = new GroupMessage { Id = Guid.NewGuid(), Group = group, GroupId = groupId };
        var message2 = new GroupMessage { Id = Guid.NewGuid(), Group = group, GroupId = groupId };
        var messages = new List<GroupMessage> { message1, message2 }.AsQueryable();
        var dbSetMock = messages.BuildMockDbSet();
        _dbContextMock.SetupGet(x => x.GroupMessages).Returns(dbSetMock.Object);

        var result = _service.GetGroupMessages(groupId);
        Assert.Contains(message1, result);
        Assert.Contains(message2, result);
    }

    [Fact]
    public void GetGroupMessagesSince_ReturnsMessagesForGroupSinceDate()
    {
        var groupId = Guid.NewGuid();
        var group = new Group { Id = groupId };
        var since = DateTime.UtcNow.AddHours(-1);
        var message1 = new GroupMessage
            { Id = Guid.NewGuid(), Group = group, GroupId = groupId, DateTime = since.AddMinutes(1) };
        var message2 = new GroupMessage
            { Id = Guid.NewGuid(), Group = group, GroupId = groupId, DateTime = since.AddMinutes(-1) };
        var messages = new List<GroupMessage> { message1, message2 }.AsQueryable();
        var dbSetMock = messages.BuildMockDbSet();
        _dbContextMock.SetupGet(x => x.GroupMessages).Returns(dbSetMock.Object);

        var result = _service.GetGroupMessagesSince(groupId, since);
        Assert.Contains(message1, result);
        Assert.DoesNotContain(message2, result);
    }

    [Fact]
    public void AddGroupEncryptionKey_AddsKeyAndSavesChanges_WhenUserAndGroupExist()
    {
        var userId = Guid.NewGuid();
        var groupId = Guid.NewGuid();
        var user = new User { Id = userId, GroupEncryptionKeys = new List<GroupEncryptionKey>() };
        var group = new Group { Id = groupId };
        var key = "encryption-key";
        var dbSetMock = new Mock<DbSet<GroupEncryptionKey>>();
        GroupEncryptionKey? addedKey = null;
        dbSetMock.Setup(x => x.Add(It.IsAny<GroupEncryptionKey>()))
            .Callback<GroupEncryptionKey>(k => addedKey = k);
        _dbContextMock.SetupGet(x => x.GroupEncryptionKeys).Returns(dbSetMock.Object);
        _dbContextMock.Setup(x => x.SaveChanges()).Verifiable();

        var users = new List<User> { user }.AsQueryable();
        var groups = new List<Group> { group }.AsQueryable();
        var usersDbSet = users.BuildMockDbSet();
        var groupsDbSet = groups.BuildMockDbSet();
        _dbContextMock.SetupGet(x => x.Users).Returns(usersDbSet.Object);
        _dbContextMock.SetupGet(x => x.Groups).Returns(groupsDbSet.Object);

        _service.AddGroupEncryptionKey(userId, groupId, key);
        Assert.NotNull(addedKey);
        Assert.Equal(user, addedKey.User);
        Assert.Equal(group, addedKey.Group);
        Assert.Equal(key, addedKey.AesKey);
        _dbContextMock.Verify(x => x.SaveChanges(), Times.Once);
    }

    [Fact]
    public void AddGroupEncryptionKey_DoesNothing_WhenUserDoesNotExist()
    {
        var userId = Guid.NewGuid();
        var groupId = Guid.NewGuid();
        var key = "encryption-key";
        var users = new List<User>().AsQueryable();
        var groups = new List<Group> { new Group { Id = groupId } }.AsQueryable();
        var usersDbSet = users.BuildMockDbSet();
        var groupsDbSet = groups.BuildMockDbSet();
        _dbContextMock.SetupGet(x => x.Users).Returns(usersDbSet.Object);
        _dbContextMock.SetupGet(x => x.Groups).Returns(groupsDbSet.Object);

        var dbSetMock = new Mock<DbSet<GroupEncryptionKey>>();
        _dbContextMock.SetupGet(x => x.GroupEncryptionKeys).Returns(dbSetMock.Object);

        _service.AddGroupEncryptionKey(userId, groupId, key);
        dbSetMock.Verify(x => x.Add(It.IsAny<GroupEncryptionKey>()), Times.Never);
        _dbContextMock.Verify(x => x.SaveChanges(), Times.Never);
    }

    [Fact]
    public void AddGroupEncryptionKey_DoesNothing_WhenGroupDoesNotExist()
    {
        var userId = Guid.NewGuid();
        var groupId = Guid.NewGuid();
        var user = new User { Id = userId, GroupEncryptionKeys = new List<GroupEncryptionKey>() };
        var key = "encryption-key";
        var users = new List<User> { user }.AsQueryable();
        var groups = new List<Group>().AsQueryable();
        var usersDbSet = users.BuildMockDbSet();
        var groupsDbSet = groups.BuildMockDbSet();
        _dbContextMock.SetupGet(x => x.Users).Returns(usersDbSet.Object);
        _dbContextMock.SetupGet(x => x.Groups).Returns(groupsDbSet.Object);

        var dbSetMock = new Mock<DbSet<GroupEncryptionKey>>();
        _dbContextMock.SetupGet(x => x.GroupEncryptionKeys).Returns(dbSetMock.Object);

        _service.AddGroupEncryptionKey(userId, groupId, key);
        dbSetMock.Verify(x => x.Add(It.IsAny<GroupEncryptionKey>()), Times.Never);
        _dbContextMock.Verify(x => x.SaveChanges(), Times.Never);
    }

    private static EntityEntry<T> CreateEntityEntry<T>(T entity) where T : class
    {
        var options = new DbContextOptionsBuilder<FakeContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var context = new FakeContext(options);
        return context.Entry(entity);
    }

    private class FakeContext(DbContextOptions<FakeContext> options) : DbContext(options)
    {
        public DbSet<Group> Groups { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserToken> UserTokens { get; set; }
        public DbSet<TotpToken> TotpTokens { get; set; }
        public DbSet<UserLoginAttempt> UserLoginAttempts { get; set; }
        public DbSet<HoneypotLoginAttempt> HoneypotLoginAttempts { get; set; }
        public DbSet<GroupMessage> GroupMessages { get; set; }
        public DbSet<GroupEncryptionKey> GroupEncryptionKeys { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Group>();
            modelBuilder.Entity<User>();
            modelBuilder.Entity<UserToken>();
            modelBuilder.Entity<TotpToken>();
            modelBuilder.Entity<UserLoginAttempt>();
            modelBuilder.Entity<HoneypotLoginAttempt>();
            modelBuilder.Entity<GroupMessage>();
            modelBuilder.Entity<GroupEncryptionKey>();
        }
    }

    // ... analogiczne testy dla CreateTotpToken,
    // LogLoginAttempt(User), LogLoginAttempt(string), GenerateUserToken, AddFriend, CreateGroup, ChangeUserDisplayName,
    // GetUserPublicKey, SaveUserKeys, SendGroupMessage
}

// Pomocnicza klasa do mockowania DbSet<T> z IQueryable
public static class DbSetMockingExtensions
{
    public static Mock<DbSet<T>> BuildMockDbSet<T>(this IQueryable<T> data) where T : class
    {
        var mockSet = new Mock<DbSet<T>>();
        mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(data.Provider);
        mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(data.Expression);
        mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(data.ElementType);
        mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());
        return mockSet;
    }
}