namespace Cryptie.Server.Features.Messages;

public interface IMessageHubService
{
    public void SendMessageToGroup(Guid group, string message);
    void SendMessageToGroup(Guid group, Guid senderId, string message);
}