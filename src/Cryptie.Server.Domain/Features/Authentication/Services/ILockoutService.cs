using Cryptie.Server.Domain.Features.Authentication.Entities;

namespace Cryptie.Server.Domain.Features.Authentication.Services;

public interface ILockoutService
{
    bool IsUserLockedOut(User? user, string honeypotLogin = "");

    bool IsUserAccountHasLock(User user, DateTime referenceLockTimestamp);

    bool IsUserAccountHasLock(string user, DateTime referenceLockTimestamp);

    bool IsUserAccountHasTooManyAttempts(User user, DateTime referenceAttemptTimestamp);

    bool IsUserAccountHasTooManyAttempts(string user, DateTime referenceAttemptTimestamp);

    void LockUserAccount(User user);

    void LockUserAccount(string user);
}