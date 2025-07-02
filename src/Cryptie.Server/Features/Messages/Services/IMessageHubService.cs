namespace Cryptie.Server.Features.Messages.Services;

public interface IMessageHubService
{
    void SendMessageToGroup(Guid group, Guid senderId, string message);
}