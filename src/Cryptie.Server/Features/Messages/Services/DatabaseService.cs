using Cryptie.Common.Entities.User;
using Cryptie.Common.Features.Messages.Services;
using Server.Persistence.DatabaseContext;

namespace Server.Features.Messages.Services;

public class DatabaseService(IAppDbContext appDbContext) : IDatabaseService
{
    public User? FindUserByToken(Guid token)
    {
        var userToken = appDbContext.UserTokens.Find(token);
        return userToken?.User;
    }
}