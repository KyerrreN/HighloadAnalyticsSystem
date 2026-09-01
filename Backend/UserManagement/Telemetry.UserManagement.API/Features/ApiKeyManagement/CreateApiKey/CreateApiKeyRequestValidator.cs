using FluentValidation;

namespace Telemetry.UserManagement.API.Features.ApiKeyManagement.CreateApiKey;

public sealed class CreateApiKeyRequestValidator : AbstractValidator<CreateApiKeyRequest>
{
    public CreateApiKeyRequestValidator(TimeProvider timeProvider)
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("'{PropertyName}' is required and cannot be empty.")
            .MinimumLength(3)
            .WithMessage("'{PropertyName}' must be at least {MinLength} characters long. You entered {TotalLength} characters.")
            .MaximumLength(100)
            .WithMessage("'{PropertyName}' must not exceed {MaxLength} characters.");

        RuleFor(x => x.ExpiresAtUtc)
            .GreaterThan(_ => timeProvider.GetUtcNow().UtcDateTime)
            .WithMessage("'{PropertyName}' must be a future date in UTC. Provided value was {PropertyValue}.")
            .LessThanOrEqualTo(_ => timeProvider.GetUtcNow().UtcDateTime.AddMonths(6))
            .WithMessage("'{PropertyName}' cannot be set further than 6 months into the future. Provided value was {PropertyValue}.");
    }
}
