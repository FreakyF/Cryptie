using Cryptie.Common.Entities;
using Cryptie.Server.Persistence.DatabaseContext;
using Microsoft.EntityFrameworkCore;

namespace Cryptie.Server.Services;

public class DatabaseService(IAppDbContext appDbContext) : IDatabaseService
{
    /// <summary>
    /// Retrieves a user associated with the specified session token.
    /// </summary>
    /// <param name="guid">Token identifying the user session.</param>
    /// <returns>The <see cref="User"/> or <c>null</c> when no token is found.</returns>
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
    /// Finds a user by their unique identifier.
    /// </summary>
    /// <param name="id">User identifier.</param>
    /// <returns>The <see cref="User"/> instance or <c>null</c> if not found.</returns>
    public User? FindUserById(Guid id)
    {
        var user = appDbContext.Users.Find(id);
        return user;
    }

    /// <summary>
    /// Retrieves a user using their login name.
    /// </summary>
    /// <param name="login">Login string.</param>
    /// <returns>The matching <see cref="User"/> or <c>null</c>.</returns>
    public User? FindUserByLogin(string login)
    {
        var user = appDbContext.Users.FirstOrDefault(u => u.Login == login);
        return user;
    }

    /// <summary>
    /// Gets a group with its members populated by identifier.
    /// </summary>
    /// <param name="id">Group identifier.</param>
    /// <returns>The found <see cref="Group"/> or <c>null</c>.</returns>
    public Group? FindGroupById(Guid id)
    {
        return appDbContext.Groups
            .Include(g => g.Members)
            .SingleOrDefault(g => g.Id == id);
    }

    /// <summary>
    /// Creates a new group and assigns the provided user as the first member.
    /// </summary>
    /// <param name="user">User that will own the group.</param>
    /// <param name="name">Name of the group.</param>
    /// <returns>The created <see cref="Group"/> entity.</returns>
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
    /// Adds the specified user to a group.
    /// </summary>
    /// <param name="user">Identifier of the user.</param>
    /// <param name="group">Identifier of the group.</param>
    public void AddUserToGroup(Guid user, Guid group)
    {
        var userToAdd = appDbContext.Users.SingleOrDefault(u => u.Id == user);
        if (userToAdd == null) return;
        appDbContext.Groups.SingleOrDefault(g => g.Id == group)?.Members.Add(userToAdd);
        appDbContext.SaveChanges();
    }

    /// <summary>
    /// Removes a user from a group.
    /// </summary>
    /// <param name="user">Identifier of the user.</param>
    /// <param name="group">Identifier of the group.</param>
    public void RemoveUserFromGroup(Guid user, Guid group)
    {
        var userToRemove = appDbContext.Users.SingleOrDefault(u => u.Id == user);
        if (userToRemove == null) return;
        appDbContext.Groups.SingleOrDefault(g => g.Id == group)?.Members.Remove(userToRemove);
        appDbContext.SaveChanges();
    }

    /// <summary>
    /// Deletes a group with the given identifier.
    /// </summary>
    /// <param name="groupGuid">Group identifier.</param>
    /// <returns><c>true</c> when group existed and was removed.</returns>
    public bool DeleteGroup(Guid groupGuid)
    {
        var group = appDbContext.Groups.SingleOrDefault(g => g.Id == groupGuid);
        if (group == null) return false;
        appDbContext.Groups.Remove(group);
        appDbContext.SaveChanges();
        return true;
    }

    /// <summary>
    /// Changes the name of a group.
    /// </summary>
    /// <param name="groupGuid">Group identifier.</param>
    /// <param name="name">New group name.</param>
    /// <returns><c>true</c> when the group exists and was updated.</returns>
    public bool ChangeGroupName(Guid groupGuid, string name)
    {
        var group = appDbContext.Groups.SingleOrDefault(g => g.Id == groupGuid);
        if (group == null) return false;

        appDbContext.Groups.SingleOrDefault(g => g.Id == groupGuid)!.Name = name;
        appDbContext.SaveChanges();

        return true;
    }

    /// <summary>
    /// Creates a temporary TOTP token for the specified user.
    /// </summary>
    /// <param name="user">User for whom the token is generated.</param>
    /// <returns>The identifier of the created token.</returns>
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
    /// Records a login attempt for the given user.
    /// </summary>
    /// <param name="user">User that attempted to log in.</param>
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
    /// Records a login attempt made with a username that does not correspond to
    /// a real user account.
    /// </summary>
    /// <param name="user">The honeypot username.</param>
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
    /// Generates and persists a new session token for the specified user.
    /// </summary>
    /// <param name="user">User for which the token is generated.</param>
    /// <returns>Identifier of the created session token.</returns>
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
    /// Adds another user to the given user's friend list.
    /// </summary>
    /// <param name="user">User who will receive the friend.</param>
    /// <param name="friend">User to be added.</param>
    public void AddFriend(User user, User friend)
    {
        user.Friends.Add(friend);
        appDbContext.SaveChanges();
    }

    /// <summary>
    /// Creates a group with the provided name.
    /// </summary>
    /// <param name="name">Group name.</param>
    /// <param name="isPrivate">Indicates whether the group is private.</param>
    /// <returns>The created <see cref="Group"/> entity.</returns>
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
    /// Updates the display name of the specified user.
    /// </summary>
    /// <param name="user">User to update.</param>
    /// <param name="name">New display name.</param>
    public void ChangeUserDisplayName(User user, string name)
    {
        user.DisplayName = name;

        appDbContext.SaveChanges();
    }

    /// <summary>
    /// Retrieves the public key of a user.
    /// </summary>
    /// <param name="userId">User identifier.</param>
    /// <returns>The public key or an empty string if the user doesn't exist.</returns>
    public string GetUserPublicKey(Guid userId)
    {
        var user = appDbContext.Users
            .AsTracking()
            .FirstOrDefault(u => u.Id == userId);

        return user?.PublicKey ?? string.Empty;
    }

    /// <summary>
    /// Saves the provided key pair for a user.
    /// </summary>
    /// <param name="user">User to update.</param>
    /// <param name="privateKey">Private key value.</param>
    /// <param name="publicKey">Public key value.</param>
    public void SaveUserKeys(User user, string privateKey, string publicKey)
    {
        user.PrivateKey = privateKey;
        user.PublicKey = publicKey;

        appDbContext.SaveChanges();
    }

    /// <summary>
    /// Stores a new message in a group and returns the created entity.
    /// </summary>
    /// <param name="group">Destination group.</param>
    /// <param name="sender">User sending the message.</param>
    /// <param name="message">Message content.</param>
    /// <returns>The stored <see cref="GroupMessage"/>.</returns>
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
    /// Retrieves a specific message from a group.
    /// </summary>
    /// <param name="messageId">Identifier of the message.</param>
    /// <param name="groupId">Identifier of the group.</param>
    /// <returns>The <see cref="GroupMessage"/> if found, otherwise <c>null</c>.</returns>
    public GroupMessage? GetGroupMessage(Guid messageId, Guid groupId)
    {
        return appDbContext.GroupMessages
            .AsTracking()
            .Include(m => m.Group)
            .Include(m => m.Sender)
            .FirstOrDefault(m => m.Id == messageId && m.GroupId == groupId);
    }

    /// <summary>
    /// Returns all messages from the specified group ordered by date.
    /// </summary>
    /// <param name="groupId">Identifier of the group.</param>
    /// <returns>List of group messages.</returns>
    public List<GroupMessage> GetGroupMessages(Guid groupId)
    {
        return appDbContext.GroupMessages
            .AsNoTracking()
            .Where(m => m.GroupId == groupId)
            .OrderBy(m => m.DateTime)
            .ToList();
    }

    /// <summary>
    /// Retrieves messages from a group newer than the specified timestamp.
    /// </summary>
    /// <param name="groupId">Group identifier.</param>
    /// <param name="since">Earliest message date to include.</param>
    /// <returns>List of messages posted after <paramref name="since"/>.</returns>
    public List<GroupMessage> GetGroupMessagesSince(Guid groupId, DateTime since)
    {
        return appDbContext.GroupMessages
            .AsNoTracking()
            .Where(m => m.GroupId == groupId && m.DateTime > since)
            .OrderBy(m => m.DateTime)
            .ToList();
    }

    /// <summary>
    /// Stores an encryption key for a user and group pair.
    /// </summary>
    /// <param name="userId">Identifier of the user.</param>
    /// <param name="groupId">Identifier of the group.</param>
    /// <param name="key">AES key to store.</param>
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
    /// Retrieves the stored encryption key for a user in a specific group.
    /// </summary>
    /// <param name="userId">Identifier of the user.</param>
    /// <param name="groupId">Identifier of the group.</param>
    /// <returns>The AES key or an empty string when missing.</returns>
    public string getGroupEncryptionKey(Guid userId, Guid groupId)
    {
        var key = appDbContext.GroupEncryptionKeys
            .FirstOrDefault(ge => ge.UserId == userId && ge.GroupId == groupId);

        return key?.AesKey ?? string.Empty;
    }
}