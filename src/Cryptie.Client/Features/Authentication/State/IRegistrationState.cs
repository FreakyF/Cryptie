using Cryptie.Common.Features.Authentication.DTOs;

namespace Cryptie.Client.Features.Authentication.State;

public interface IRegistrationState
{
    RegisterResponseDto? LastResponse { get; set; }
}