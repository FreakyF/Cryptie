using Cryptie.Common.Features.Authentication.DTOs;

namespace Cryptie.Client.Features.Authentication.State;

public class RegistrationState : IRegistrationState
{
    public RegisterResponseDto? LastResponse { get; set; }
}