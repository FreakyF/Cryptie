using Cryptie.Common.Features.Authentication.DTOs;

namespace Cryptie.Client.Desktop.Features.Authentication.State;

public interface IRegistrationState
{
    RegisterResponseDto? LastResponse { get; set; }
}