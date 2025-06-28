namespace Cryptie.Client.Features.Chats.ViewModels;

public sealed class ChatMessageViewModel(string message, bool isOwn, string groupName)
{
    public string Message { get; } = message;
    public bool IsOwn { get; } = isOwn;
    public string GroupName { get; } = groupName;
}