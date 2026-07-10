using System.Security.Cryptography;
using System.Text;

namespace Telemetry.Read.Domain.Utils;

public static class HashUtils
{
    public static string HashApiKey(string apiKey)
    {
        if (string.IsNullOrWhiteSpace(apiKey)) return "unknown";

        byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(apiKey));

        return Convert.ToHexString(bytes)[..16].ToLowerInvariant();
    }
}
