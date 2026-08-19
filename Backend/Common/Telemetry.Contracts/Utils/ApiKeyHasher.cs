using System.Security.Cryptography;
using System.Text;
using Telemetry.Contracts.Interfaces;

namespace Telemetry.Contracts.Utils;

public class ApiKeyHasher : IApiKeyHasher
{
    /// <summary>
    /// Computes SHA-256 hash of a project api key
    /// </summary>
    public string HashKey(string rawKey)
    {
        var bytes = Encoding.UTF8.GetBytes(rawKey);
        var hashBytes = SHA256.HashData(bytes);
        return Convert.ToHexStringLower(hashBytes);
    }
}
