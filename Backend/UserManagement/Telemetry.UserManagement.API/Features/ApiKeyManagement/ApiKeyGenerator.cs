using System.Security.Cryptography;
using System.Text;

namespace Telemetry.UserManagement.API.Features.ApiKeyManagement;

public class ApiKeyGenerator : IApiKeyGenerator
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
    /// Computes SHA-256 hash of a project api key
    /// </summary>
    public string HashKey(string rawKey)
    {
        var bytes = Encoding.UTF8.GetBytes(rawKey);
        var hashBytes = SHA256.HashData(bytes);
        return Convert.ToHexStringLower(hashBytes);
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
    string HashKey(string rawKey);
    string CreateDisplayPrefix(string rawKey);
}
