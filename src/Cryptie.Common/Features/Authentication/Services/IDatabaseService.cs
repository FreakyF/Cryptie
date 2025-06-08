using Cryptie.Common.Entities.User;

namespace Cryptie.Common.Features.Authentication.Services;

public interface IDatabaseService
{
    Guid CreateTotpToken(User user);
    void LogLoginAttempt(User user);
    void LogLoginAttempt(string user);
    Guid GenerateUserToken(User user);
}