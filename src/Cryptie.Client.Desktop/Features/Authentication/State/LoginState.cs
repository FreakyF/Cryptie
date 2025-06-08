using Cryptie.Common.Features.Authentication.DTOs;



public class LoginState : ILoginState
{
    public LoginResponseDto? LastResponse { get; set; }
}