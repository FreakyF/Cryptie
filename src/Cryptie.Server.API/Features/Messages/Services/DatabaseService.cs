using Cryptie.Server.Domain.Features.Authentication.Entities.User;
using Cryptie.Server.Domain.Features.Messages;
using Cryptie.Server.Infrastructure.Persistence.DatabaseContext;

namespace Cryptie.Server.API.Features.Messages.Services;

public class DatabaseService(IAppDbContext appDbContext) : IDatabaseService
{
    public User? FindUserByToken(Guid token)
    {
        var userToken = appDbContext.UserTokens.Find(token);
        return userToken?.User;
    }
}