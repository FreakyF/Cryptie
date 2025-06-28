using Cryptie.Client.Features.Authentication.State;
using Cryptie.Client.Features.Groups.State;
using Cryptie.Client.Features.Menu.State;

namespace Cryptie.Client.Features.Account.Dependencies;

public record AccountState(
    IUserState UserState,
    IGroupSelectionState GroupSelectionState,
    ILoginState LoginState,
    IRegistrationState RegistrationState);