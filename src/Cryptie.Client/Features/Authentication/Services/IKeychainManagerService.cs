using System.Diagnostics.CodeAnalysis;

namespace Cryptie.Client.Features.Authentication.Services;

public interface IKeychainManagerService
{
    bool TrySaveSessionToken(string token, [NotNullWhen(false)] out string? errorMessage);
    bool TryGetSessionToken([NotNullWhen(true)] out string? token, [NotNullWhen(false)] out string? errorMessage);
    bool TryClearSessionToken([NotNullWhen(false)] out string? errorMessage);
    bool TrySavePrivateKey(string privateKey, [NotNullWhen(false)] out string? errorMessage);
    bool TryGetPrivateKey([NotNullWhen(true)] out string? privateKey, [NotNullWhen(false)] out string? errorMessage);
    bool TryClearPrivateKey([NotNullWhen(false)] out string? errorMessage);
}