using Cryptie.Server.Domain.Features.Authentication.Entities.User;

namespace Cryptie.Server.Domain.Features.Authentication.Services;

public interface IDatabaseService
{
    Guid CreateTotpToken(User user);
    void LogLoginAttempt(User user);
    void LogLoginAttempt(string user);
    Guid GenerateUserToken(User user);
}