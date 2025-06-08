using Cryptie.Common.Features.Authentication.DTOs;



public class RegistrationState : IRegistrationState
{
    public RegisterResponseDto? LastResponse { get; set; }
}