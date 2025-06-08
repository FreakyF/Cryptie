using Cryptie.Common.Features.Authentication.DTOs;

namespace Cryptie.Client.Desktop.Features.Authentication.State;

public class LoginState : ILoginState
{
    public LoginResponseDto? LastResponse { get; set; }
}