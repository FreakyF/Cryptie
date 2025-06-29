using Cryptie.Client.Configuration;
using Cryptie.Client.Features.Chats.Services;
using Cryptie.Client.Features.ChatSettings.ViewModels;
using Cryptie.Client.Features.Groups.Dependencies;
using Cryptie.Client.Features.Groups.Services;
using Cryptie.Client.Features.Groups.State;
using Microsoft.Extensions.Options;

namespace Cryptie.Client.Features.Chats.Dependencies;

public sealed record ChatsViewModelDependencies(
    IOptions<ClientOptions> Options,
    AddFriendDependencies AddFriendDependencies,
    IGroupService GroupService,
    IGroupSelectionState GroupState,
    IMessagesService MessagesService,
    ChatSettingsViewModel SettingsPanel);