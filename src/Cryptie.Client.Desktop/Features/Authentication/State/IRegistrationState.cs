using Cryptie.Common.Features.Authentication.DTOs;



public interface IRegistrationState
{
    RegisterResponseDto? LastResponse { get; set; }
}