namespace Cryptie.Client.Application.Features.Authentication.Services;

public interface IAuthenticationService
{
    Task RegisterAsync(RegisterRequest registerRequest);
}