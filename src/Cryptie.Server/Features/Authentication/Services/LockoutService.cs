using Cryptie.Common.Entities;
using Cryptie.Server.Persistence.DatabaseContext;

namespace Cryptie.Server.Features.Authentication.Services;

public class LockoutService(IAppDbContext appDbContext) : ILockoutService
{
    /// <summary>
    ///     Determines whether a user or honeypot account should be locked out
    ///     based on failed login attempts.
    /// </summary>
    /// <param name="user">Real user instance if available.</param>
    /// <param name="honeypotLogin">Login of a honeypot account when no real user exists.</param>
    /// <returns><c>true</c> if the account is locked out; otherwise <c>false</c>.</returns>
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
    ///     Checks if the specified user has an active lockout entry.
    /// </summary>
    /// <param name="user">User entity to check.</param>
    /// <param name="referenceLockTimestamp">Timestamp defining the lockout window.</param>
    /// <returns><c>true</c> when the user has an active lock.</returns>
    public bool IsUserAccountHasLock(User user, DateTime referenceLockTimestamp)
    {
        return appDbContext.UserAccountLocks.Any(l => l.User == user && l.Until > referenceLockTimestamp);
    }

    /// <summary>
    ///     Checks if the honeypot account has an active lockout entry.
    /// </summary>
    /// <param name="user">Honeypot login to check.</param>
    /// <param name="referenceLockTimestamp">Timestamp defining the lockout window.</param>
    /// <returns><c>true</c> when the account has an active lock.</returns>
    public bool IsUserAccountHasLock(string user, DateTime referenceLockTimestamp)
    {
        return appDbContext.HoneypotAccountLocks.Any(l => l.Username == user && l.Until > referenceLockTimestamp);
    }

    /// <summary>
    ///     Evaluates if the user has exceeded the allowed login attempts.
    /// </summary>
    /// <param name="user">User entity to check.</param>
    /// <param name="referenceAttemptTimestamp">Timestamp defining the attempts window.</param>
    /// <returns><c>true</c> if there are too many attempts.</returns>
    public bool IsUserAccountHasTooManyAttempts(User user, DateTime referenceAttemptTimestamp)
    {
        return appDbContext.UserLoginAttempts.Count(a => a.User == user && a.Timestamp > referenceAttemptTimestamp) <
               2;
    }

    /// <summary>
    ///     Evaluates if the honeypot account has exceeded the allowed login attempts.
    /// </summary>
    /// <param name="user">Honeypot login to check.</param>
    /// <param name="referenceAttemptTimestamp">Timestamp defining the attempts window.</param>
    /// <returns><c>true</c> if there are too many attempts.</returns>
    public bool IsUserAccountHasTooManyAttempts(string user, DateTime referenceAttemptTimestamp)
    {
        return appDbContext.HoneypotLoginAttempts.Count(a =>
            a.Username == user && a.Timestamp > referenceAttemptTimestamp) < 2;
    }

    /// <summary>
    ///     Creates a lockout entry for the specified user.
    /// </summary>
    /// <param name="user">User entity to lock.</param>
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
    ///     Creates a lockout entry for the specified honeypot login.
    /// </summary>
    /// <param name="user">Honeypot login to lock.</param>
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