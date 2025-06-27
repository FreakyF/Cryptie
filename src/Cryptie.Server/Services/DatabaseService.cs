using Cryptie.Common.Entities;
using Cryptie.Server.Persistence.DatabaseContext;
using Microsoft.EntityFrameworkCore;

namespace Cryptie.Server.Services;

public class DatabaseService(IAppDbContext appDbContext) : IDatabaseService
{
    public User? GetUserFromToken(Guid guid)
    {
        return appDbContext.UserTokens
            .AsTracking()
            .Where(t => t.Id == guid)
            .Include(t => t.User)
            .ThenInclude(u => u!.Groups)
            .FirstOrDefault()?
            .User;
    }

    public User? FindUserById(Guid id)
    {
        var user = appDbContext.Users.Find(id);
        return user;
    }

    public User? FindUserByLogin(string login)
    {
        var user = appDbContext.Users.FirstOrDefault(u => u.Login == login);
        return user;
    }

    public Group? CreateNewGroup(User user, string name)
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

    public void AddUserToGroup(Guid user, Guid group)
    {
        var userToAdd = appDbContext.Users.SingleOrDefault(u => u.Id == user);
        if (userToAdd == null) return;
        appDbContext.Groups.SingleOrDefault(g => g.Id == group)?.Members.Add(userToAdd);
        appDbContext.SaveChanges();
    }

    public void RemoveUserFromGroup(Guid user, Guid group)
    {
        var userToRemove = appDbContext.Users.SingleOrDefault(u => u.Id == user);
        if (userToRemove == null) return;
        appDbContext.Groups.SingleOrDefault(g => g.Id == group)?.Members.Remove(userToRemove);
        appDbContext.SaveChanges();
    }

    public bool DeleteGroup(Guid groupGuid)
    {
        var group = appDbContext.Groups.SingleOrDefault(g => g.Id == groupGuid);
        if (group == null) return false;
        appDbContext.Groups.Remove(group);
        appDbContext.SaveChanges();
        return true;
    }

    public bool ChangeGroupName(Guid groupGuid, string name)
    {
        var group = appDbContext.Groups.SingleOrDefault(g => g.Id == groupGuid);
        if (group == null) return false;

        appDbContext.Groups.SingleOrDefault(g => g.Id == groupGuid)!.Name = name;
        appDbContext.SaveChanges();

        return true;
    }

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

    public void AddFriend(User user, User friend)
    {
        user.Friends.Add(friend);
        appDbContext.SaveChanges();
    }

    public Group CreateGroup(string name)
    {
        var group = appDbContext.Groups.Add(new Group
        {
            Name = name
        });

        appDbContext.SaveChanges();

        return group.Entity;
    }

    public void ChangeUserDisplayName(User user, string name)
    {
        user.DisplayName = name;

        appDbContext.SaveChanges();
    }
}