using Cryptie.Common.Entities.Honeypot;
using Cryptie.Common.Entities.LoginPolicy;
using Cryptie.Common.Entities.User;
using Cryptie.Common.Features.Authentication.Services;
using Cryptie.Server.Persistence.DatabaseContext;

namespace Cryptie.Server.Features.Authentication.Services;

public class LockoutService(AppDbContext appDbContext) : ILockoutService
{
    public bool IsUserLockedOut(User? user, string honeypotLogin = "")
    {
        var referenceLockTimestamp = DateTime.UtcNow.AddMinutes(-60);
        var referenceAttemptTimestamp = DateTime.UtcNow.AddMinutes(-15);

        if (user != null)
        {
            if (IsUserAccountHasLock(user, referenceLockTimestamp))
            {
                return true;
            }

            if (IsUserAccountHasTooManyAttempts(user, referenceAttemptTimestamp))
            {
                return false;
            }

            LockUserAccount(user);
            return true;
        }

        if (IsUserAccountHasLock(honeypotLogin, referenceLockTimestamp))
        {
            return true;
        }

        if (IsUserAccountHasTooManyAttempts(honeypotLogin, referenceAttemptTimestamp))
        {
            return false;
        }

        LockUserAccount(honeypotLogin);
        return true;
    }

    public bool IsUserAccountHasLock(User user, DateTime referenceLockTimestamp)
    {
        return appDbContext.UserAccountLocks.Any(l => l.User == user && l.Until > referenceLockTimestamp);
    }

    public bool IsUserAccountHasLock(string user, DateTime referenceLockTimestamp)
    {
        return appDbContext.UserAccountHoneypotLocks.Any(l => l.User == user && l.Until > referenceLockTimestamp);
    }

    public bool IsUserAccountHasTooManyAttempts(User user, DateTime referenceAttemptTimestamp)
    {
        return appDbContext.UserLoginAttempts.Count(a => a.User == user && a.TimeStamp > referenceAttemptTimestamp) <
               2;
    }

    public bool IsUserAccountHasTooManyAttempts(string user, DateTime referenceAttemptTimestamp)
    {
        return appDbContext.UserLoginHoneypotAttempts.Count(a =>
            a.User == user && a.TimeStamp > referenceAttemptTimestamp) < 2;
    }

    public void LockUserAccount(User user)
    {
        appDbContext.UserAccountLocks.Add(new UserAccountLock
        {
            Id = Guid.Empty,
            Until = DateTime.UtcNow.AddMinutes(60),
            User = user
        });

        appDbContext.SaveChanges();
    }

    public void LockUserAccount(string user)
    {
        appDbContext.UserAccountHoneypotLocks.Add(new UserAccountHoneypotLock
        {
            Id = Guid.Empty,
            Until = DateTime.UtcNow.AddMinutes(60),
            User = user
        });

        appDbContext.SaveChanges();
    }
}