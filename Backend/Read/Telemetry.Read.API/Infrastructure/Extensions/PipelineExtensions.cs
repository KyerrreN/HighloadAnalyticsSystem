using FluentValidation;
using System.Reflection;
using Telemetry.Read.Domain.Abstractions;
using Telemetry.Read.Domain.Abstractions.Decorator;

namespace Telemetry.Read.API.Infrastructure.Extensions;

public static class PipelineExtensions
{
    extension (IServiceCollection services)
    {
        public IServiceCollection AddQueryPipeline(params Assembly[] assembliesToScan)
        {
            services.AddValidatorsFromAssemblies(assembliesToScan);

            services.Scan(scan => scan
                .FromAssemblies(assembliesToScan)
                .AddClasses(classes => classes.AssignableTo(typeof(IQueryHandler<,>)))
                .AsImplementedInterfaces()
                .WithScopedLifetime());

            // Pipeline
            services.Decorate(typeof(IQueryHandler<,>), typeof(CachingQueryDecorator<,>));

            services.Decorate(typeof(IQueryHandler<,>), typeof(ValidationDecorator<,>));

            return services;
        }
    }
}
