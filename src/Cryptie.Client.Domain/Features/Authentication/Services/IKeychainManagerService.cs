namespace Cryptie.Client.Domain.Features.Authentication.Services;

public interface IKeychainManagerService
{
    bool TrySaveSessionToken(string token, out string? errorMessage);
    bool TryGetSessionToken(out string? token, out string? errorMessage);
    bool TryClearSessionToken(out string? errorMessage);
}