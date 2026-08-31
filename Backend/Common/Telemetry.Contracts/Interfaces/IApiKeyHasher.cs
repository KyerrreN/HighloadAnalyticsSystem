namespace Telemetry.Contracts.Interfaces;

public interface IApiKeyHasher
{
    string HashKey(string rawKey);
}
