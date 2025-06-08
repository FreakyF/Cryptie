using Cryptie.Common.Entities.Honeypot;
using Cryptie.Common.Entities.LoginPolicy;
using Cryptie.Common.Entities.SessionTokens;
using Cryptie.Common.Entities.User;
using Cryptie.Common.Features.Authentication.Services;
using Cryptie.Server.Persistence.DatabaseContext;

namespace Cryptie.Server.Features.Authentication.Services;

public class DatabaseService(AppDbContext appDbContext) : IDatabaseService
{
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
            TimeStamp = DateTime.UtcNow,
            User = user
        });

        appDbContext.SaveChanges();
    }

    public void LogLoginAttempt(string user)
    {
        appDbContext.UserLoginHoneypotAttempts.Add(new UserLoginHoneypotAttempt
        {
            Id = Guid.Empty,
            TimeStamp = DateTime.UtcNow,
            User = user
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
}