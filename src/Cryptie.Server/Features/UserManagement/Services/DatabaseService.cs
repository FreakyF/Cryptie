using Cryptie.Common.Entities.User;
using Cryptie.Server.Persistence.DatabaseContext;
using Microsoft.EntityFrameworkCore;

namespace Cryptie.Server.Features.UserManagement.Services;

public class DatabaseService(AppDbContext appDbContext)
{
    public User? GetUserFromToken(Guid guid)
    {
        var userGuid = appDbContext.UserTokens.Include(userToken => userToken.User).SingleOrDefault(t => t.Id == guid);

        return userGuid?.User ?? null;
    }
}