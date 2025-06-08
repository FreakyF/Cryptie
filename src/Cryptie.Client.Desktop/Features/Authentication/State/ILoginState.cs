using Cryptie.Common.Features.Authentication.DTOs;



public interface ILoginState
{
    LoginResponseDto? LastResponse { get; set; }
}