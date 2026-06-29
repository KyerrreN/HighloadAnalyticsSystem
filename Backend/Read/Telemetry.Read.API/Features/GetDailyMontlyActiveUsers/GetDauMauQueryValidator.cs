using FluentValidation;

namespace Telemetry.Read.API.Features.GetDailyMontlyActiveUsers;

public class GetDauMauQueryValidator : AbstractValidator<GetDauMauQuery>
{
    public GetDauMauQueryValidator(TimeProvider timeProvider)
    {
        RuleFor(x => x.ProjectApiKey)
            .NotEmpty()
            .WithMessage("{PropertyName} is required");

        RuleFor(x => x.From)
            .Must(from => from >= timeProvider.GetUtcNow().UtcDateTime.AddMonths(-1).AddDays(-1))
            .WithMessage("{PropertyName} cannot be earlier than 1 month and 1 day from current system time");

        When(x => x.To.HasValue, () =>
        {
            RuleFor(x => x.To!.Value)
                .LessThanOrEqualTo(timeProvider.GetUtcNow().UtcDateTime)
                .WithMessage("\"{PropertyName}\" cannot be past current system time");

            RuleFor(x => x.To!.Value)
                .GreaterThanOrEqualTo(x => x.From)
                .WithMessage("\"{PropertyName}\" must be greater than or equal to From");
        });
    }
}
