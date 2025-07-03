using Cryptie.Common.Entities;
using Cryptie.Server.Persistence.DatabaseContext;

namespace Cryptie.Server.Features.Authentication.Services;

public class LockoutService(IAppDbContext appDbContext) : ILockoutService
{
    /// <summary>
    /// Determines whether the specified user or honeypot account should be locked out
    /// based on recent login attempts.
    /// </summary>
    /// <param name="user">The real user to check.</param>
    /// <param name="honeypotLogin">Login string used when user is null.</param>
    /// <returns><c>true</c> if access should be denied.</returns>
    public bool IsUserLockedOut(User? user, string honeypotLogin = "")
    {
        var referenceLockTimestamp = DateTime.UtcNow.AddMinutes(-60);
        var referenceAttemptTimestamp = DateTime.UtcNow.AddMinutes(-15);

        if (user != null)
        {
            if (IsUserAccountHasLock(user, referenceLockTimestamp)) return true;

            if (IsUserAccountHasTooManyAttempts(user, referenceAttemptTimestamp)) return false;

            LockUserAccount(user);
            return true;
        }

        if (IsUserAccountHasLock(honeypotLogin, referenceLockTimestamp)) return true;

        if (IsUserAccountHasTooManyAttempts(honeypotLogin, referenceAttemptTimestamp)) return false;

        LockUserAccount(honeypotLogin);
        return true;
    }

    /// <summary>
    /// Checks whether a user account currently has an active lock.
    /// </summary>
    /// <param name="user">User account.</param>
    /// <param name="referenceLockTimestamp">Timestamp of oldest valid lock.</param>
    /// <returns><c>true</c> if a lock exists.</returns>
    public bool IsUserAccountHasLock(User user, DateTime referenceLockTimestamp)
    {
        return appDbContext.UserAccountLocks.Any(l => l.User == user && l.Until > referenceLockTimestamp);
    }

    /// <summary>
    /// Checks whether a honeypot login has an active lock.
    /// </summary>
    /// <param name="user">The honeypot login name.</param>
    /// <param name="referenceLockTimestamp">Timestamp of oldest valid lock.</param>
    /// <returns><c>true</c> if locked.</returns>
    public bool IsUserAccountHasLock(string user, DateTime referenceLockTimestamp)
    {
        return appDbContext.HoneypotAccountLocks.Any(l => l.Username == user && l.Until > referenceLockTimestamp);
    }

    /// <summary>
    /// Checks if a user has exceeded the allowed number of login attempts.
    /// </summary>
    /// <param name="user">User account.</param>
    /// <param name="referenceAttemptTimestamp">Only attempts newer than this are counted.</param>
    /// <returns><c>true</c> when attempts are within limit.</returns>
    public bool IsUserAccountHasTooManyAttempts(User user, DateTime referenceAttemptTimestamp)
    {
        return appDbContext.UserLoginAttempts.Count(a => a.User == user && a.Timestamp > referenceAttemptTimestamp) <
               2;
    }

    /// <summary>
    /// Checks if a honeypot login has too many login attempts.
    /// </summary>
    /// <param name="user">The honeypot login name.</param>
    /// <param name="referenceAttemptTimestamp">Only attempts newer than this are counted.</param>
    /// <returns><c>true</c> when attempts are within limit.</returns>
    public bool IsUserAccountHasTooManyAttempts(string user, DateTime referenceAttemptTimestamp)
    {
        return appDbContext.HoneypotLoginAttempts.Count(a =>
            a.Username == user && a.Timestamp > referenceAttemptTimestamp) < 2;
    }

    /// <summary>
    /// Adds a lock entry for the specified user.
    /// </summary>
    /// <param name="user">User to lock.</param>
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

    /// <summary>
    /// Adds a lock entry for a honeypot login.
    /// </summary>
    /// <param name="user">Login string to lock.</param>
    public void LockUserAccount(string user)
    {
        appDbContext.HoneypotAccountLocks.Add(new HoneypotAccountLock
        {
            Id = Guid.Empty,
            Until = DateTime.UtcNow.AddMinutes(60),
            Username = user
        });

        appDbContext.SaveChanges();
    }
}