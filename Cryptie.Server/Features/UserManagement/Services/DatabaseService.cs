using Cryptie.Common.Entities.User;
using Microsoft.EntityFrameworkCore;
using Server.Persistence.DatabaseContext;

namespace Server.Features.UserManagement.Services;

public class DatabaseService(AppDbContext appDbContext)
{
    public User? GetUserFromToken(Guid guid)
    {
        var userGuid = appDbContext.UserTokens.Include(userToken => userToken.User).SingleOrDefault(t => t.Id == guid);

        return userGuid?.User ?? null;
    }
}