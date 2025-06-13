using Cryptie.Common.Entities.Group;
using Cryptie.Common.Entities.User;
using Cryptie.Server.Persistence.DatabaseContext;
using Microsoft.EntityFrameworkCore;

namespace Cryptie.Server.Features.GroupManagment.Services;

public class DatabaseService(AppDbContext appDbContext)
{
    public User? GetUserFromToken(Guid guid)
    {
        var userGuid = appDbContext.UserTokens.Include(userToken => userToken.User).SingleOrDefault(t => t.Id == guid);

        return userGuid?.User ?? null;
    }
    
    public User? FindUserById(Guid id)
    {
        var user = appDbContext.Users.Find(id);
        return user;
    }

    public Group? CreateNewGroup(User user, string name)
    {
        var newGroup = new Group
        {
            Id = Guid.Empty,
            Name = name,
            Keys = {  },
            Messages = {  },
            Users = { user }
        };

        var createdGroup = appDbContext.Groups.Add(newGroup);
        appDbContext.SaveChanges();
        
        return createdGroup.Entity;
    }

    public void AddUserToGroup(Guid user, Guid group)
    {
        var userToAdd = appDbContext.Users.SingleOrDefault(u => u.Id == user);
        if (userToAdd != null) appDbContext.Groups.SingleOrDefault(g => g.Id == group)?.Users.Add(userToAdd); //TODO xd
        appDbContext.SaveChanges();
    }
}