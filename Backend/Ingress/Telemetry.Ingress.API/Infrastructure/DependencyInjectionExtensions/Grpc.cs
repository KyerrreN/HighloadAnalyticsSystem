using Microsoft.Extensions.Options;
using Telemetry.Contracts.Grpc;
using Telemetry.Ingress.API.Infrastructure.Options;

namespace Telemetry.Ingress.API.Infrastructure.DependencyInjectionExtensions;

public static class Grpc
{
    extension (IServiceCollection services)
    {
        public IServiceCollection ConfigureGrpc(IConfiguration configuration)
        {
            services.AddGrpcClient<ApiKeyValidation.ApiKeyValidationClient>((sp, options) =>
            {
                var grpcOptions = sp.GetRequiredService<IOptions<GrpcOptions>>().Value;
                options.Address = new Uri(grpcOptions.UserManagementUrl);
            });

            return services;
        }
    }
}
