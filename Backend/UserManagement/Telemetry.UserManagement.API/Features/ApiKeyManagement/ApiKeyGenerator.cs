using System.Security.Cryptography;

namespace Telemetry.UserManagement.API.Features.ApiKeyManagement;

public sealed class ApiKeyGenerator : IApiKeyGenerator
{
    private const string KeyPrefix = "pk_";

    /// <summary>
    /// Generates raw key + 64 hex characters
    /// </summary>
    public string GenerateRawKey()
    {
        var randomBytes = RandomNumberGenerator.GetBytes(32);
        var secretHex = Convert.ToHexStringLower(randomBytes);
        return $"{KeyPrefix}{secretHex}";
    }

    /// <summary>
    /// Generate safe-to-show display prefix
    /// </summary>
    public string CreateDisplayPrefix(string rawKey)
    {
        if (rawKey.Length <= 8) return KeyPrefix;
        return $"{KeyPrefix}...{rawKey[^4..]}";
    }
}

public interface IApiKeyGenerator
{
    string GenerateRawKey();
    string CreateDisplayPrefix(string rawKey);
}
