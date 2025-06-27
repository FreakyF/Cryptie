using Cryptie.Common.Entities;

namespace Cryptie.Server.Services;

public interface IDatabaseService
{
    public User? GetUserFromToken(Guid guid);
    public User? FindUserById(Guid id);
    public User? FindUserByLogin(string login);
    public Group? CreateNewGroup(User user, string name);
    public void AddUserToGroup(Guid user, Guid group);
    public void RemoveUserFromGroup(Guid user, Guid group);
    public bool DeleteGroup(Guid groupGuid);
    public bool ChangeGroupName(Guid groupGuid, string name);
    public Guid CreateTotpToken(User user);
    public void LogLoginAttempt(User user);
    public void LogLoginAttempt(string user);
    public Guid GenerateUserToken(User user);
    public void AddFriend(User user, User friend);
    public Group CreateGroup(string name);
    public void ChangeUserDisplayName(User user, string name);
}