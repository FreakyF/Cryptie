using Cryptie.Common.Entities;
using Cryptie.Server.Persistence.DatabaseContext;
using Microsoft.EntityFrameworkCore;

namespace Cryptie.Server.Services;

public class DatabaseService(IAppDbContext appDbContext) : IDatabaseService
{
    /// <summary>
    ///     Retrieves a user entity from a session token.
    /// </summary>
    /// <param name="guid">Token identifier.</param>
    /// <returns>User entity or <c>null</c> when not found.</returns>
    public User? GetUserFromToken(Guid guid)
    {
        return appDbContext.UserTokens
            .AsTracking()
            .Where(t => t.Id == guid)
            .Include(t => t.User)
            .ThenInclude(u => u.Groups)
            .FirstOrDefault()?
            .User;
    }

    /// <summary>
    ///     Finds a user by its unique identifier.
    /// </summary>
    /// <param name="id">User identifier.</param>
    /// <returns>User entity or <c>null</c>.</returns>
    public User? FindUserById(Guid id)
    {
        var user = appDbContext.Users.Find(id);
        return user;
    }

    /// <summary>
    ///     Locates a user by login name.
    /// </summary>
    /// <param name="login">Login name.</param>
    /// <returns>User entity or <c>null</c>.</returns>
    public User? FindUserByLogin(string login)
    {
        var user = appDbContext.Users.FirstOrDefault(u => u.Login == login);
        return user;
    }

    /// <summary>
    ///     Retrieves a group by its identifier including member list.
    /// </summary>
    /// <param name="id">Group identifier.</param>
    /// <returns>Group entity or <c>null</c>.</returns>
    public Group? FindGroupById(Guid id)
    {
        return appDbContext.Groups
            .Include(g => g.Members)
            .SingleOrDefault(g => g.Id == id);
    }

    /// <summary>
    ///     Creates a new group and assigns the specified user as a member.
    /// </summary>
    /// <param name="user">Initial member.</param>
    /// <param name="name">Name of the group.</param>
    /// <returns>The newly created group.</returns>
    public Group CreateNewGroup(User user, string name)
    {
        var newGroup = new Group
        {
            Id = Guid.Empty,
            Name = name,
            Members = { user }
        };

        var createdGroup = appDbContext.Groups.Add(newGroup);
        appDbContext.SaveChanges();

        return createdGroup.Entity;
    }

    /// <summary>
    ///     Adds an existing user to an existing group.
    /// </summary>
    /// <param name="user">User identifier.</param>
    /// <param name="group">Group identifier.</param>
    public void AddUserToGroup(Guid user, Guid group)
    {
        var userToAdd = appDbContext.Users.SingleOrDefault(u => u.Id == user);
        if (userToAdd == null) return;
        appDbContext.Groups.SingleOrDefault(g => g.Id == group)?.Members.Add(userToAdd);
        appDbContext.SaveChanges();
    }

    /// <summary>
    ///     Removes a user from a group.
    /// </summary>
    /// <param name="user">User identifier.</param>
    /// <param name="group">Group identifier.</param>
    public void RemoveUserFromGroup(Guid user, Guid group)
    {
        var userToRemove = appDbContext.Users.SingleOrDefault(u => u.Id == user);
        if (userToRemove == null) return;
        appDbContext.Groups.SingleOrDefault(g => g.Id == group)?.Members.Remove(userToRemove);
        appDbContext.SaveChanges();
    }

    /// <summary>
    ///     Deletes a group from the database.
    /// </summary>
    /// <param name="groupGuid">Group identifier.</param>
    /// <returns><c>true</c> if removed; otherwise <c>false</c>.</returns>
    public bool DeleteGroup(Guid groupGuid)
    {
        var group = appDbContext.Groups.SingleOrDefault(g => g.Id == groupGuid);
        if (group == null) return false;
        appDbContext.Groups.Remove(group);
        appDbContext.SaveChanges();
        return true;
    }

    /// <summary>
    ///     Changes the name of a group.
    /// </summary>
    /// <param name="groupGuid">Group identifier.</param>
    /// <param name="name">New name.</param>
    /// <returns><c>true</c> if the group exists and was updated.</returns>
    public bool ChangeGroupName(Guid groupGuid, string name)
    {
        var group = appDbContext.Groups.SingleOrDefault(g => g.Id == groupGuid);
        if (group == null) return false;

        appDbContext.Groups.SingleOrDefault(g => g.Id == groupGuid)!.Name = name;
        appDbContext.SaveChanges();

        return true;
    }

    /// <summary>
    ///     Creates a TOTP token entity for the user.
    /// </summary>
    /// <param name="user">User to associate the token with.</param>
    /// <returns>Identifier of the created token.</returns>
    public Guid CreateTotpToken(User user)
    {
        var totpToken = appDbContext.TotpTokens.Add(new TotpToken
        {
            Id = Guid.Empty,
            User = user,
            Until = DateTime.UtcNow.AddMinutes(5)
        });

        appDbContext.SaveChanges();

        return totpToken.Entity.Id;
    }

    /// <summary>
    ///     Logs a failed login attempt for a real user.
    /// </summary>
    /// <param name="user">User entity.</param>
    public void LogLoginAttempt(User user)
    {
        appDbContext.UserLoginAttempts.Add(new UserLoginAttempt
        {
            Id = Guid.Empty,
            Timestamp = DateTime.UtcNow,
            User = user
        });

        appDbContext.SaveChanges();
    }

    /// <summary>
    ///     Logs a failed login attempt for a honeypot account.
    /// </summary>
    /// <param name="user">Honeypot login name.</param>
    public void LogLoginAttempt(string user)
    {
        appDbContext.HoneypotLoginAttempts.Add(new HoneypotLoginAttempt
        {
            Id = Guid.Empty,
            Timestamp = DateTime.UtcNow,
            Username = user
        });

        appDbContext.SaveChanges();
    }

    /// <summary>
    ///     Generates and persists a new session token for the user.
    /// </summary>
    /// <param name="user">User entity.</param>
    /// <returns>Identifier of the new token.</returns>
    public Guid GenerateUserToken(User user)
    {
        var token = appDbContext.UserTokens.Add(new UserToken
        {
            Id = Guid.Empty,
            User = user
        });

        appDbContext.SaveChanges();

        return token.Entity.Id;
    }

    /// <summary>
    ///     Adds a friend relationship between two users.
    /// </summary>
    /// <param name="user">User initiating the friendship.</param>
    /// <param name="friend">Friend user.</param>
    public void AddFriend(User user, User friend)
    {
        user.Friends.Add(friend);
        appDbContext.SaveChanges();
    }

    /// <summary>
    ///     Creates a new group without assigning members.
    /// </summary>
    /// <param name="name">Name of the group.</param>
    /// <param name="isPrivate">Whether the group is private.</param>
    /// <returns>The created group.</returns>
    public Group CreateGroup(string name, bool isPrivate = false)
    {
        var group = appDbContext.Groups.Add(new Group
        {
            Name = name,
            IsPrivate = isPrivate
        });

        appDbContext.SaveChanges();

        return group.Entity;
    }

    /// <summary>
    ///     Updates the display name of the specified user.
    /// </summary>
    /// <param name="user">User entity.</param>
    /// <param name="name">New display name.</param>
    public void ChangeUserDisplayName(User user, string name)
    {
        user.DisplayName = name;

        appDbContext.SaveChanges();
    }

    /// <summary>
    ///     Retrieves stored public key of a user.
    /// </summary>
    /// <param name="userId">User identifier.</param>
    /// <returns>Public key string or empty when not found.</returns>
    public string GetUserPublicKey(Guid userId)
    {
        var user = appDbContext.Users
            .AsTracking()
            .FirstOrDefault(u => u.Id == userId);

        return user?.PublicKey ?? string.Empty;
    }

    /// <summary>
    ///     Stores new key pair for the user.
    /// </summary>
    /// <param name="user">User entity.</param>
    /// <param name="privateKey">Private key string.</param>
    /// <param name="publicKey">Public key string.</param>
    public void SaveUserKeys(User user, string privateKey, string publicKey)
    {
        user.PrivateKey = privateKey;
        user.PublicKey = publicKey;

        appDbContext.SaveChanges();
    }

    /// <summary>
    ///     Persists a new message in the specified group.
    /// </summary>
    /// <param name="group">Target group.</param>
    /// <param name="sender">Sender user.</param>
    /// <param name="message">Message content.</param>
    /// <returns>The created <see cref="GroupMessage"/> entity.</returns>
    public GroupMessage SendGroupMessage(Group group, User sender, string message)
    {
        var groupMessage = new GroupMessage
        {
            Id = Guid.Empty,
            Message = message,
            DateTime = DateTime.UtcNow,
            Group = group,
            Sender = sender
        };

        appDbContext.GroupMessages.Add(groupMessage);
        appDbContext.SaveChanges();

        return groupMessage;
    }

    /// <summary>
    ///     Retrieves a single message by id from a group.
    /// </summary>
    /// <param name="messageId">Message identifier.</param>
    /// <param name="groupId">Group identifier.</param>
    /// <returns>Message entity or <c>null</c>.</returns>
    public GroupMessage? GetGroupMessage(Guid messageId, Guid groupId)
    {
        return appDbContext.GroupMessages
            .AsTracking()
            .Include(m => m.Group)
            .Include(m => m.Sender)
            .FirstOrDefault(m => m.Id == messageId && m.GroupId == groupId);
    }

    /// <summary>
    ///     Retrieves all messages for the specified group ordered by date.
    /// </summary>
    /// <param name="groupId">Group identifier.</param>
    /// <returns>List of messages.</returns>
    public List<GroupMessage> GetGroupMessages(Guid groupId)
    {
        return appDbContext.GroupMessages
            .AsNoTracking()
            .Where(m => m.GroupId == groupId)
            .OrderBy(m => m.DateTime)
            .ToList();
    }

    /// <summary>
    ///     Retrieves all messages for a group after the specified timestamp.
    /// </summary>
    /// <param name="groupId">Group identifier.</param>
    /// <param name="since">Earliest timestamp.</param>
    /// <returns>List of messages.</returns>
    public List<GroupMessage> GetGroupMessagesSince(Guid groupId, DateTime since)
    {
        return appDbContext.GroupMessages
            .AsNoTracking()
            .Where(m => m.GroupId == groupId && m.DateTime > since)
            .OrderBy(m => m.DateTime)
            .ToList();
    }

    /// <summary>
    ///     Adds an AES encryption key for a user-group pair.
    /// </summary>
    /// <param name="userId">User identifier.</param>
    /// <param name="groupId">Group identifier.</param>
    /// <param name="key">Encryption key.</param>
    public void AddGroupEncryptionKey(Guid userId, Guid groupId, string key)
    {
        var user = appDbContext.Users
            .Include(u => u.GroupEncryptionKeys)
            .FirstOrDefault(u => u.Id == userId);

        if (user == null) return;

        var group = appDbContext.Groups
            .FirstOrDefault(g => g.Id == groupId);

        if (group == null) return;

        var newKey = new GroupEncryptionKey
        {
            Id = Guid.Empty,
            User = user,
            Group = group,
            AesKey = key
        };

        appDbContext.GroupEncryptionKeys.Add(newKey);
        appDbContext.SaveChanges();
    }

    /// <summary>
    ///     Retrieves the AES encryption key for a user-group pair.
    /// </summary>
    /// <param name="userId">User identifier.</param>
    /// <param name="groupId">Group identifier.</param>
    /// <returns>Encryption key string or empty when not set.</returns>
    public string getGroupEncryptionKey(Guid userId, Guid groupId)
    {
        var key = appDbContext.GroupEncryptionKeys
            .FirstOrDefault(ge => ge.UserId == userId && ge.GroupId == groupId);

        return key?.AesKey ?? string.Empty;
    }
}