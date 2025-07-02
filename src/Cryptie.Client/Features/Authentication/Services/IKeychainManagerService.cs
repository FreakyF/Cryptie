using System.Diagnostics.CodeAnalysis;

namespace Cryptie.Client.Features.Authentication.Services;

// ReSharper disable OutParameterValueIsAlwaysDiscarded.Global
public interface IKeychainManagerService
{
    bool TrySaveSessionToken(string token, [NotNullWhen(false)] out string? errorMessage);
    bool TryGetSessionToken([NotNullWhen(true)] out string? token, [NotNullWhen(false)] out string? errorMessage);
    void TryClearSessionToken([NotNullWhen(false)] out string? errorMessage);
    bool TrySavePrivateKey(string privateKey, [NotNullWhen(false)] out string? errorMessage);
    bool TryGetPrivateKey([NotNullWhen(true)] out string? privateKey, [NotNullWhen(false)] out string? errorMessage);
    void TryClearPrivateKey([NotNullWhen(false)] out string? errorMessage);
}