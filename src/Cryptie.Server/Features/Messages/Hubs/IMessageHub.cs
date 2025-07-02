namespace Cryptie.Server.Features.Messages;

public interface IMessageHub
{
    Task SendMessage(string message);
    Task JoinGroup(Guid user, Guid group);
    Task SendMessageToGroup(Guid group, Guid senderId, string message);
}