using FluentValidation;

namespace Telemetry.UserManagement.API.Features.ProjectManagement.CreateProject;

public sealed class CreateProjectRequestDtoValidator : AbstractValidator<CreateProjectRequestDto>
{
    public CreateProjectRequestDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("'{PropertyName}' is required and cannot be empty.")
            .MinimumLength(3)
            .WithMessage("'{PropertyName}' must be at least {MinLength} characters long. You entered {TotalLength} characters.")
            .MaximumLength(100)
            .WithMessage("'{PropertyName}' must not exceed {MaxLength} characters.");

        RuleFor(x => x.Description)
            .MaximumLength(500)
            .WithMessage("'{PropertyName}' must not exceed {MaxLength} characters.")
            .When(x => !string.IsNullOrEmpty(x.Description));
    }
}
