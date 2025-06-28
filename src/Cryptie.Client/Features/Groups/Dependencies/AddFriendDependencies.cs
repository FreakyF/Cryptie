using Cryptie.Client.Features.AddFriend.Services;
using Cryptie.Client.Features.Authentication.Services;
using Cryptie.Client.Features.Menu.State;
using Cryptie.Common.Features.UserManagement.DTOs;
using FluentValidation;

namespace Cryptie.Client.Features.Groups.Dependencies;

// ReSharper disable once ClassNeverInstantiated.Global
public sealed record AddFriendDependencies(
    IFriendsService FriendsService,
    IKeychainManagerService KeychainManager,
    IValidator<AddFriendRequestDto> Validator,
    IUserState UserState);