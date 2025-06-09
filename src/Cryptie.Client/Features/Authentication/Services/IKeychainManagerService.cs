using System.Diagnostics.CodeAnalysis;

namespace Cryptie.Client.Features.Authentication.Services;

public interface IKeychainManagerService
{
    bool TrySaveSessionToken(string token, [NotNullWhen(false)] out string? errorMessage);
    bool TryGetSessionToken(out string? token, [NotNullWhen(false)] out string? errorMessage);
    bool TryClearSessionToken([NotNullWhen(false)] out string? errorMessage);
}