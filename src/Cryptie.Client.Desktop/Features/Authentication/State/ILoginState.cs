using Cryptie.Common.Features.Authentication.DTOs;

namespace Cryptie.Client.Desktop.Features.Authentication.State;

public interface ILoginState
{
    LoginResponseDto? LastResponse { get; set; }
}