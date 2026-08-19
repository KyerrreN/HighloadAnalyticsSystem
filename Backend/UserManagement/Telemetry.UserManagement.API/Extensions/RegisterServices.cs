using Telemetry.Contracts.Interfaces;
using Telemetry.Contracts.Utils;
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
            services.AddScoped<IApiKeyManagementService, ApiKeyManagementService>();

            services.AddSingleton<IApiKeyHasher, ApiKeyHasher>();
            services.AddSingleton<IApiKeyGenerator, ApiKeyGenerator>();

            return services;
        }
    }
}
