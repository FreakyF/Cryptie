using Cryptie.Common.Entities.Group;
using Cryptie.Common.Entities.User;

namespace Cryptie.Server.Services;

public interface IDatabaseService
{
    public User? GetUserFromToken(Guid guid);
    public User? FindUserById(Guid id);
    public Group? CreateNewGroup(User user, string name);
    public void AddUserToGroup(Guid user, Guid group);
    public void RemoveUserFromGroup(Guid user, Guid group);
    public bool DeleteGroup(Guid groupGuid);
    public bool ChangeGroupName(Guid groupGuid, string name);
    public Guid CreateTotpToken(User user);
    public void LogLoginAttempt(User user);
    public void LogLoginAttempt(string user);
    public Guid GenerateUserToken(User user);
}