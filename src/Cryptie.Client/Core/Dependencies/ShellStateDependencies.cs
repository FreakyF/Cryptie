using Cryptie.Client.Features.Authentication.State;
using Cryptie.Client.Features.Groups.State;
using Cryptie.Client.Features.Menu.State;

namespace Cryptie.Client.Core.Dependencies;

public sealed record ShellStateDependencies(
    IUserState UserState,
    IGroupSelectionState GroupSelectionState,
    ILoginState LoginState,
    IRegistrationState RegistrationState
);