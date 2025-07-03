using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using KeySharp;
using static System.Int32;

namespace Cryptie.Client.Features.Authentication.Services;

public class KeychainManagerService : IKeychainManagerService
{
    private const string ProductName = "com.cryptie.client";

    private const string ServiceNameSession = "SessionService";
    private const string AccountSession = "CurrentUser";

    private const string ServiceNamePrivateKey = "PrivateKeyService";
    private const string MetaAccount = "CurrentUser_KeyChunks";
    private const string ChunkAccountPrefix = "CurrentUser_Chunk_";
    private const int ChunkSize = 1024;

    /// <summary>
    ///     Stores the session token in the operating system keychain.
    /// </summary>
    /// <param name="token">Token to persist.</param>
    /// <param name="errorMessage">Outputs an error message when the operation fails.</param>
    /// <returns><c>true</c> when the token was saved successfully.</returns>
    public bool TrySaveSessionToken(string token, [NotNullWhen(false)] out string? errorMessage)
    {
        errorMessage = null;
        if (string.IsNullOrWhiteSpace(token))
        {
            errorMessage = "Session token cannot be null or empty.";
            return false;
        }

        try
        {
            Keyring.SetPassword(ProductName, ServiceNameSession, AccountSession, token);
            return true;
        }
        catch (KeyringException ex)
        {
            errorMessage = $"Failed to save session token: {ex.Message}";
            return false;
        }
    }

    /// <summary>
    ///     Attempts to retrieve the session token from the keychain.
    /// </summary>
    public bool TryGetSessionToken([NotNullWhen(true)] out string? token, [NotNullWhen(false)] out string? errorMessage)
    {
        token = null;
        errorMessage = null;

        try
        {
            token = Keyring.GetPassword(ProductName, ServiceNameSession, AccountSession);
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

    /// <summary>
    ///     Removes the session token from the keychain.
    /// </summary>
    public void TryClearSessionToken([NotNullWhen(false)] out string? errorMessage)
    {
        errorMessage = null;

        try
        {
            Keyring.DeletePassword(ProductName, ServiceNameSession, AccountSession);
        }
        catch (KeyringException ex)
        {
            errorMessage = $"Failed to clear session token: {ex.Message}";
        }
    }

    /// <summary>
    ///     Saves the user's private key in the keychain, chunking it if necessary.
    /// </summary>
    public bool TrySavePrivateKey(string privateKey, [NotNullWhen(false)] out string? errorMessage)
    {
        errorMessage = null;
        if (string.IsNullOrWhiteSpace(privateKey))
        {
            errorMessage = "Private key cannot be null or empty.";
            return false;
        }

        var parts = new List<string>();
        for (var i = 0; i < privateKey.Length; i += ChunkSize)
        {
            var length = Math.Min(ChunkSize, privateKey.Length - i);
            parts.Add(privateKey.Substring(i, length));
        }

        try
        {
            Keyring.SetPassword(
                ProductName,
                ServiceNamePrivateKey,
                MetaAccount,
                parts.Count.ToString()
            );
        }
        catch (KeyringException ex)
        {
            errorMessage = $"Failed to save private key metadata: {ex.Message}";
            return false;
        }

        for (var idx = 0; idx < parts.Count; idx++)
        {
            try
            {
                Keyring.SetPassword(
                    ProductName,
                    ServiceNamePrivateKey,
                    ChunkAccountPrefix + idx,
                    parts[idx]
                );
            }
            catch (KeyringException ex)
            {
                errorMessage = $"Failed to save private key chunk #{idx}: {ex.Message}";
                return false;
            }
        }

        return true;
    }

    /// <summary>
    ///     Retrieves the private key from the keychain.
    /// </summary>
    public bool TryGetPrivateKey([NotNullWhen(true)] out string? privateKey,
        [NotNullWhen(false)] out string? errorMessage)
    {
        privateKey = null;
        errorMessage = null;

        int count;
        try
        {
            var countStr = Keyring.GetPassword(
                ProductName,
                ServiceNamePrivateKey,
                MetaAccount
            );
            if (string.IsNullOrEmpty(countStr) || !TryParse(countStr, out count))
            {
                errorMessage = "No private key metadata found.";
                return false;
            }
        }
        catch (KeyringException ex)
        {
            errorMessage = $"Failed to retrieve private key metadata: {ex.Message}";
            return false;
        }

        var sb = new StringBuilder();
        for (var idx = 0; idx < count; idx++)
        {
            try
            {
                var part = Keyring.GetPassword(
                    ProductName,
                    ServiceNamePrivateKey,
                    ChunkAccountPrefix + idx
                );
                if (part == null)
                {
                    errorMessage = $"Missing private key chunk #{idx}.";
                    return false;
                }

                sb.Append(part);
            }
            catch (KeyringException ex)
            {
                errorMessage = $"Failed to retrieve private key chunk #{idx}: {ex.Message}";
                return false;
            }
        }

        privateKey = sb.ToString();
        return true;
    }

    /// <summary>
    ///     Removes the stored private key from the keychain.
    /// </summary>
    public void TryClearPrivateKey([NotNullWhen(false)] out string? errorMessage)
    {
        errorMessage = null;

        var count = 0;
        try
        {
            var countStr = Keyring.GetPassword(
                ProductName,
                ServiceNamePrivateKey,
                MetaAccount
            );
            if (!string.IsNullOrEmpty(countStr) && !TryParse(countStr, out count))
            {
                count = 0;
            }
        }
        catch
        {
            // Swallow exception: do nothing
        }

        for (var i = 0; i < count; i++)
        {
            try
            {
                Keyring.DeletePassword(
                    ProductName,
                    ServiceNamePrivateKey,
                    ChunkAccountPrefix + i
                );
            }
            catch
            {
                // Swallow exception: do nothing
            }
        }

        try
        {
            Keyring.DeletePassword(
                ProductName,
                ServiceNamePrivateKey,
                MetaAccount
            );
        }
        catch (KeyringException ex)
        {
            errorMessage = $"Failed to clear private key metadata: {ex.Message}";
        }
    }
}