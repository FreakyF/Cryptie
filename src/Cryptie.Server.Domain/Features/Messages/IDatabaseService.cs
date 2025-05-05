using Cryptie.Server.Domain.Features.Authentication.Entities.User;

namespace Cryptie.Server.Domain.Features.Messages;

public interface IDatabaseService
{
    public User? FindUserByToken(Guid token);
}