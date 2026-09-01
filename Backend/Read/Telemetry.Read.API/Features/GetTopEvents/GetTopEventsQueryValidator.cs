using FluentValidation;

namespace Telemetry.Read.API.Features.GetTopEvents;

public sealed class GetTopEventsQueryValidator : AbstractValidator<GetTopEventsQuery>
{
    public GetTopEventsQueryValidator()
    {
        RuleFor(x => x.ProjectId)
            .NotEmpty()
            .WithMessage("'{PropertyName}' is required and cannot be empty.");

        RuleFor(x => x.Period)
            .IsInEnum()
            .WithMessage("The specified value for '{PropertyName}' is invalid.");

        RuleFor(x => x.Limit)
            .InclusiveBetween(1, 100)
            .WithMessage("'{PropertyName}' must be between {From} and {To}. You entered: {PropertyValue}.");
    }
}
