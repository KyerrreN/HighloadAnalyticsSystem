using Telemetry.UserManagement.API.Features.ApiKeyManagement;
using Telemetry.UserManagement.API.Features.ProjectManagement;
using Telemetry.UserManagement.API.Features.Shared.Keycloak;
using Telemetry.UserManagement.API.Features.UserManagement;

namespace Telemetry.UserManagement.API.Extensions;

public static class RegisterServices
{
    extension (IServiceCollection services)
    {
        public IServiceCollection RegisterCustomServices()
        {
            services.AddScoped<IKeycloakAdminService, KeycloakAdminService>();
            services.AddScoped<IUserManagementService, UserManagementService>();
            services.AddScoped<IProjectManagementService, ProjectManagementService>();
            services.AddScoped<IApiKeyGenerator, ApiKeyGenerator>();
            services.AddScoped<IApiKeyManagementService, ApiKeyManagementService>();

            return services;
        }
    }
}
