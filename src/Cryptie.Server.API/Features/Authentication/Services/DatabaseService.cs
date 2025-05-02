using Cryptie.Server.Domain.Features.Authentication.Entities.Honeypot;
using Cryptie.Server.Domain.Features.Authentication.Entities.LoginPolicy;
using Cryptie.Server.Domain.Features.Authentication.Entities.SessionTokens;
using Cryptie.Server.Domain.Features.Authentication.Entities.User;
using Cryptie.Server.Domain.Features.Authentication.Services;
using Cryptie.Server.Infrastructure.Persistence.DatabaseContext;

namespace Cryptie.Server.API.Features.Authentication.Services;

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