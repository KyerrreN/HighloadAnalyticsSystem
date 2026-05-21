using Telemetry.Read.CrossCuttingConcerns.Abstractions;
using Telemetry.Read.CrossCuttingConcerns.Abstractions.Decorator;

namespace Telemetry.Read.API.Infrastructure.Extensions;

public static class PipelineExtensions
{
    extension (IServiceCollection services)
    {
        public IServiceCollection AddQueryPipeline()
        {
            services.Scan(scan => scan
                .FromAssemblies(typeof(Program).Assembly)
                .AddClasses(classes => classes.AssignableTo(typeof(IQueryHandler<,>)))
                .AsImplementedInterfaces()
                .WithScopedLifetime());

            // Pipeline
            services.Decorate(typeof(IQueryHandler<,>), typeof(CachingQueryDecorator<,>));

            return services;
        }
    }
}
