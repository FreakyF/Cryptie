using Cryptie.Client.Features.AddFriend.Services;
using Cryptie.Client.Features.Menu.State;
using Cryptie.Common.Features.UserManagement.DTOs;
using FluentValidation;

namespace Cryptie.Client.Features.Groups.Dependencies;

public sealed record AddFriendDependencies(
    IFriendsService FriendsService,
    IValidator<AddFriendRequestDto> Validator,
    IUserState UserState);