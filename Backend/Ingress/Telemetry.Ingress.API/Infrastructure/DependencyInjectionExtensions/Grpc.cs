using Telemetry.Contracts.Grpc;

namespace Telemetry.Ingress.API.Infrastructure.DependencyInjectionExtensions;

public static class Grpc
{
    extension (IServiceCollection services)
    {
        public IServiceCollection ConfigureGrpc(IConfiguration configuration)
        {
            services.AddGrpcClient<ApiKeyValidation.ApiKeyValidationClient>(options =>
            {
                string url = configuration["GrpcServices:UserManagementUrl"]
                    ?? "http://localhost:5001"; // todo: options

                options.Address = new Uri(url);
            });

            return services;
        }
    }
}
