namespace Cryptie.Server.Features.Messages;

public interface IMessageHubService
{
    void SendMessageToGroup(Guid group, Guid senderId, string message);
}