using FluentValidation;
using System.Reflection;
using Telemetry.Read.Domain.Abstractions;
using Telemetry.Read.Domain.Abstractions.Decorator;
using Telemetry.Read.Domain.Abstractions.Enrichers;

namespace Telemetry.Read.API.Infrastructure.Extensions;

public static class PipelineExtensions
{
    extension (IServiceCollection services)
    {
        public IServiceCollection AddQueryPipeline(params Assembly[] assembliesToScan)
        {
            services.AddValidatorsFromAssemblies(assembliesToScan);

            // autoregister query handlers
            services.Scan(scan => scan
                .FromAssemblies(assembliesToScan)
                .AddClasses(classes => classes.AssignableTo(typeof(IQueryHandler<,>)))
                .AsImplementedInterfaces()
                .WithScopedLifetime());

            // autoregister decorators
            services.Scan(scan => scan
                .FromAssemblies(assembliesToScan)
                .AddClasses(classes => classes.AssignableTo(typeof(IActivityEnricher<>)))
                .AsImplementedInterfaces()
                .WithTransientLifetime());

            // Pipeline
            services.Decorate(typeof(IQueryHandler<,>), typeof(CachingQueryDecorator<,>));

            services.Decorate(typeof(IQueryHandler<,>), typeof(ValidationDecorator<,>));

            services.Decorate(typeof(IQueryHandler<,>), typeof(ObservabilityQueryDecorator<,>));

            return services;
        }
    }
}
