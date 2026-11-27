using FluentValidation;
using HardshipApp.Api.Contracts;

namespace HardshipApp.Api.Validation;

public sealed class CreateHardshipApplicationRequestValidator
    : AbstractValidator<CreateHardshipApplicationRequest>
{
    public const int MinimumAge = 18;

    public CreateHardshipApplicationRequestValidator(TimeProvider clock)
    {
        RuleFor(request => request.CustomerName)
            .NotEmpty().WithMessage("Customer name is required.")
            .MaximumLength(200).WithMessage("Customer name must be 200 characters or fewer.");

        RuleFor(request => request.DateOfBirth)
            .Must(dateOfBirth => dateOfBirth < Today(clock))
            .WithMessage("Date of birth must be in the past.")
            .Must(dateOfBirth => AgeOn(Today(clock), dateOfBirth) >= MinimumAge)
            .WithMessage($"Applicant must be at least {MinimumAge} years old.");

        RuleFor(request => request.MonthlyIncome)
            .GreaterThanOrEqualTo(0).WithMessage("Monthly income cannot be negative.");

        RuleFor(request => request.MonthlyExpenses)
            .GreaterThanOrEqualTo(0).WithMessage("Monthly expenses cannot be negative.");

        // Expenses are deliberately not required to be below income. A deficit is the
        // situation being reported, not a mistake in the form.

        RuleFor(request => request.HardshipReason)
            .MaximumLength(1000).WithMessage("Hardship reason must be 1000 characters or fewer.");
    }

    private static DateOnly Today(TimeProvider clock) => DateOnly.FromDateTime(clock.GetUtcNow().UtcDateTime);

    private static int AgeOn(DateOnly today, DateOnly dateOfBirth)
    {
        int age = today.Year - dateOfBirth.Year;

        if (dateOfBirth > today.AddYears(-age))
        {
            age--;
        }

        return age;
    }
}
