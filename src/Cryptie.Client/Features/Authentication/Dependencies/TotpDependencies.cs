using Cryptie.Client.Core.Mapping;
using Cryptie.Client.Features.Authentication.Services;
using Cryptie.Client.Features.Authentication.State;
using Cryptie.Client.Features.Menu.State;
using Cryptie.Common.Features.Authentication.DTOs;
using FluentValidation;
using MapsterMapper;

namespace Cryptie.Client.Features.Authentication.Dependencies;

public sealed record TotpDependencies(
    IValidator<TotpRequestDto> Validator,
    IExceptionMessageMapper ExceptionMapper,
    ILoginState LoginState,
    IMapper Mapper,
    IKeychainManagerService Keychain,
    IUserState UserState
);