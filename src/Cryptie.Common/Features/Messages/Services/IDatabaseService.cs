using Cryptie.Common.Entities.User;

namespace Cryptie.Common.Features.Messages.Services;

public interface IDatabaseService
{
    public User? FindUserByToken(Guid token);
}