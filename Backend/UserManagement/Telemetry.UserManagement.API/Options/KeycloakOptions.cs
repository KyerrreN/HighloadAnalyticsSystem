namespace Telemetry.UserManagement.API.Options;

public class KeycloakOptions
{
    public const string SectionName = "Keycloak";

    public required string Authority { get; set; }
    public required bool RequireHttpsMetadata { get; set; }
    public required string ClientId { get; set; }
    public required string ClientSecret { get; set; }

    public Uri AuthorityUri => new(Authority);
    public Uri BaseAddress => new(AuthorityUri.GetLeftPart(UriPartial.Authority));
    public string Realm => AuthorityUri.Segments.Last().Trim('/');
}
