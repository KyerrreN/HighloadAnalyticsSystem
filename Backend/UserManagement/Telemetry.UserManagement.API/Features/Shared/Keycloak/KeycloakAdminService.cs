using Microsoft.Extensions.Options;
using Refit;
using System.Net;
using Telemetry.UserManagement.API.Options;
using Telemetry.UserManagement.Infrastructure.Errors;
using Telemetry.UserManagement.Infrastructure.Result;

namespace Telemetry.UserManagement.API.Features.Shared.Keycloak;

public class KeycloakAdminService : IKeycloakAdminService
{
    private readonly IKeycloakApi _api;
    private readonly KeycloakOptions _options;

    public KeycloakAdminService(IKeycloakApi api, IOptions<KeycloakOptions> options)
    {
        _api = api;
        _options = options.Value;
    }

    public async Task<Result<string>> GetAdminAccessTokenAsync(CancellationToken ct = default)
    {
        try
        {
            var request = new KeycloakTokenRequest
            {
                ClientId = _options.ClientId,
                ClientSecret = _options.ClientSecret
            };

            var response = await _api.GetTokenAsync(_options.Realm, request, ct);
            return response.AccessToken;
        }
        catch (ApiException ex)
        {
            return Result.Failed<string>(KeycloakErrors.TokenRequestFailed((int)ex.StatusCode));
        }
        catch (Exception)
        {
            return Result.Failed<string>(KeycloakErrors.UnknownError);
        }
    }

    public async Task<Result> DeleteUserAsync(Guid userId, CancellationToken ct = default)
    {
        var tokenResult = await GetAdminAccessTokenAsync(ct);

        if (tokenResult.IsFailure)
        {
            return Result.Failed(tokenResult.Error);
        }

        try
        {
            var bearerToken = $"Bearer {tokenResult.Value}";

            await _api.DeleteUserAsync(bearerToken, _options.Realm, userId, ct);

            return Result.Success();
        }
        catch (ApiException ex) when (ex.StatusCode == HttpStatusCode.NotFound) 
        {
            return Result.Success(); // idempotency
        } 
        catch (ApiException ex)
        {
            return Result.Failed(KeycloakErrors.UserDeletionFailed((int)ex.StatusCode));
        }
        catch (Exception)
        {
            return Result.Failed(KeycloakErrors.UnknownError);
        }
    }
}

public interface IKeycloakAdminService
{
    /// <summary>
    /// Get admin access token
    /// </summary>
    /// <param name="ct"></param>
    /// <returns>
    ///     <see cref="Result{T}"/> - operation result, with access token of type <see cref="string"/>. 
    ///     <see cref="Error"/> of <see cref="Result"/> is generated automatically in case of an error
    /// </returns>
    Task<Result<string>> GetAdminAccessTokenAsync(CancellationToken ct = default);

    /// <summary>
    /// Delete user from KeyCloak
    /// </summary>
    /// <param name="userId">Id of a user</param>
    /// <param name="ct"></param>
    /// <returns>
    ///     <see cref="Result"/> - operation result.
    ///     <see cref="Error"/> of <see cref="Result"/> is generated automatically in case of an error
    /// </returns>
    Task<Result> DeleteUserAsync(Guid userId, CancellationToken ct = default);
}
