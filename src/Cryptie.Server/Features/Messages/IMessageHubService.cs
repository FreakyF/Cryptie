namespace Cryptie.Server.Features.Messages;

public interface IMessageHubService
{
    public void SendMessageToGroup(Guid group, string message);
}