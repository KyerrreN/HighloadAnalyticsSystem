using System.Security.Cryptography;
using System.Text;
using Telemetry.Contracts.Interfaces;

namespace Telemetry.Contracts.Utils;

public class ApiKeyHasher : IApiKeyHasher
{
    private const int MaxKeyLength = 128;

    public string HashKey(string rawKey)
    {
        if (string.IsNullOrEmpty(rawKey))
        {
            return string.Empty;
        }

        Span<byte> sourceBytes = stackalloc byte[MaxKeyLength];

        // fall back if a key is > MaxKeyLength
        if (!Encoding.UTF8.TryGetBytes(rawKey, sourceBytes, out int bytesWritten))
        {
            sourceBytes = Encoding.UTF8.GetBytes(rawKey);
            bytesWritten = sourceBytes.Length;
        }

        Span<byte> hashBytes = stackalloc byte[32];
        SHA256.HashData(sourceBytes[..bytesWritten], hashBytes);

        return Convert.ToHexStringLower(hashBytes);
    }
}
