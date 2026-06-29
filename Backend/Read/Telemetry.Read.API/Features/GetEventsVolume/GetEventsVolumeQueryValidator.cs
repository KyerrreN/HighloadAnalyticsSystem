using FluentValidation;

namespace Telemetry.Read.API.Features.GetEventsVolume;

public class GetEventsVolumeQueryValidator : AbstractValidator<GetEventsVolumeQuery>
{
    private const int MaxAllowedPoints = 500;

    public GetEventsVolumeQueryValidator()
    {
        RuleFor(x => x.ProjectApiKey)
            .NotEmpty()
            .WithMessage("'{PropertyName}' is required and cannot be empty.");

        RuleFor(x => x.Granularity)
            .IsInEnum()
            .WithMessage("The specified value for '{PropertyName}' is invalid.");

        RuleFor(x => x)
            .Must(x => x.From <= x.To)
            .WithName("To")
            .WithMessage("'{PropertyName}' date cannot be earlier than 'From' date.");

        RuleFor(x => x)
            .Custom((query, context) =>
            {
                int totalDays = query.To.DayNumber - query.From.DayNumber + 1;

                if (totalDays > 30)
                {
                    context.AddFailure(nameof(query.To), $"The maximum allowed date range is 30 days. You requested {totalDays} days.");
                    return;
                }

                int totalMinutes = totalDays * 24 * 60;
                int granularityMinutes = (int)query.Granularity;

                int estimatedPoints = totalMinutes / granularityMinutes;

                if (estimatedPoints > MaxAllowedPoints)
                {
                    context.AddFailure(
                        nameof(query.Granularity),
                        $"The requested combination yields {estimatedPoints} data points, which exceeds the layout limit of {MaxAllowedPoints}. Please increase 'Granularity' or shorten the date range.");
                }
            });
    }
}
