using Refit;
using System.Text.Json.Serialization;

namespace Telemetry.UserManagement.API.Features.Shared.Keycloak;

/// <summary>
/// HttpClient to work with Keycloak
/// </summary>
public interface IKeycloakApi
{
    /// <summary>
    /// Get admin access token
    /// </summary>
    /// <param name="realm">Realm (tenant) name</param>
    /// <param name="request">Request body</param>
    /// <param name="ct"></param>
    /// <returns><see cref="KeycloakTokenResponse"/> - response with Access Token</returns>
    [Post("/realms/{realm}/protocol/openid-connect/token")]
    Task<KeycloakTokenResponse> GetTokenAsync(
        string realm,
        [Body(BodySerializationMethod.UrlEncoded)] KeycloakTokenRequest request,
        CancellationToken ct = default);

    /// <summary>
    /// Deletes user from specified realm by user id
    /// </summary>
    /// <param name="bearerToken">Access token (admin)</param>
    /// <param name="realm">Realm (tenant) name</param>
    /// <param name="userId">Id of a user</param>
    /// <param name="ct"></param>
    /// <returns></returns>
    [Delete("/admin/realms/{realm}/users/{userId}")]
    Task DeleteUserAsync(
        [Header("Authorization")] string bearerToken,
        string realm,
        Guid userId,
        CancellationToken ct = default);
}

public sealed record KeycloakTokenRequest
{
    [AliasAs("grant_type")]
    public string GrantType { get; init; } = "client_credentials";

    [AliasAs("client_id")]
    public required string ClientId { get; init; }

    [AliasAs("client_secret")]
    public required string ClientSecret { get; init; }
}

public sealed record KeycloakTokenResponse([property: JsonPropertyName("access_token")] string AccessToken);