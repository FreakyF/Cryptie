using Cryptie.Client.Domain.Features.Authentication.Services;
using KeySharp;

namespace Cryptie.Client.Infrastructure.Features.Authentication.Services;

public class KeychainManagerService : IKeychainManagerService
{
    private const string ProductName = "com.cryptie.client";
    private const string ServiceName = "SessionService";
    private const string Account = "CurrentUser";

    public bool TrySaveSessionToken(string token, out string? errorMessage)
    {
        errorMessage = null;

        if (string.IsNullOrWhiteSpace(token))
        {
            errorMessage = "Session token cannot be null or empty.";
            return false;
        }

        try
        {
            Keyring.SetPassword(ProductName, ServiceName, Account, token);
            return true;
        }
        catch (KeyringException ex)
        {
            errorMessage = $"Failed to save session token: {ex.Message}";
            return false;
        }
    }

    public bool TryGetSessionToken(out string? token, out string? errorMessage)
    {
        token = null;
        errorMessage = null;

        try
        {
            token = Keyring.GetPassword(ProductName, ServiceName, Account);
            if (!string.IsNullOrEmpty(token))
            {
                return true;
            }

            errorMessage = "No session token found.";
            return false;
        }
        catch (KeyringException ex)
        {
            errorMessage = $"Failed to retrieve session token: {ex.Message}";
            return false;
        }
    }

    public bool TryClearSessionToken(out string? errorMessage)
    {
        errorMessage = null;

        try
        {
            Keyring.DeletePassword(ProductName, ServiceName, Account);
            return true;
        }
        catch (KeyringException ex)
        {
            errorMessage = $"Failed to clear session token: {ex.Message}";
            return false;
        }
    }
}