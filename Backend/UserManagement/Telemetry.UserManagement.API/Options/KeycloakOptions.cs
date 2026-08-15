using System.ComponentModel.DataAnnotations;

namespace Telemetry.UserManagement.API.Options;

public class KeycloakOptions
{
    public const string SectionName = "Keycloak";

    [Required(ErrorMessage = "Keycloak Authority URL is required.")]
    [Url(ErrorMessage = "Keycloak Authority must be a valid URL.")]
    public required string Authority { get; set; }

    [Required(ErrorMessage = "Keycloak Audience is required.")]
    [MinLength(1, ErrorMessage = "Keycloak Audience cannot be empty.")]
    public required string Audience { get; set; }

    public required bool RequireHttpsMetadata { get; set; }

    [Required(ErrorMessage = "Keycloak Client ID is required.")]
    [MinLength(1, ErrorMessage = "Keycloak Client ID cannot be empty.")]
    public required string ClientId { get; set; }

    [Required(ErrorMessage = "Keycloak Client Secret is required.")]
    [MinLength(1, ErrorMessage = "Keycloak Client Secret cannot be empty.")]
    public required string ClientSecret { get; set; }

    public Uri AuthorityUri => new(Authority);
    public Uri BaseAddress => new(AuthorityUri.GetLeftPart(UriPartial.Authority));
    public string Realm => AuthorityUri.Segments.Last().Trim('/');
}
