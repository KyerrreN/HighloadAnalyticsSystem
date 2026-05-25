using FluentValidation;

namespace Telemetry.Read.CrossCuttingConcerns.Abstractions.Decorator;

public class ValidationDecorator<TQuery, TResponse> : IQueryHandler<TQuery, TResponse>
    where TQuery : IQuery<TResponse>
{
    private readonly IQueryHandler<TQuery, TResponse> _inner;
    private readonly IEnumerable<IValidator<TQuery>> _validators;

    public ValidationDecorator(IQueryHandler<TQuery, TResponse> inner, IEnumerable<IValidator<TQuery>> validators)
    {
        _inner = inner;
        _validators = validators;
    }


    public async Task<TResponse> HandleAsync(TQuery query, CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            return await _inner.HandleAsync(query, cancellationToken);
        }

        var context = new ValidationContext<TQuery>(query);

        var validationFailures = _validators
            .Select(v => v.Validate(context))
            .SelectMany(result => result.Errors)
            .Where(f => f is not null)
            .ToList();

        if (validationFailures.Count != 0)
        {
            // todo: throw exception? or implement result pattern
            throw new ValidationException(validationFailures);
        }

        return await _inner.HandleAsync(query, cancellationToken);
    }
}
