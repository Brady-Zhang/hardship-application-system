using FluentValidation.TestHelper;
using HardshipApp.Api.Contracts;
using HardshipApp.Api.Validation;
using HardshipApp.UnitTests.Support;

namespace HardshipApp.UnitTests.Validation;

public class CreateHardshipApplicationRequestValidatorTests
{
    private static readonly DateOnly Today = new(2026, 11, 28);

    private readonly CreateHardshipApplicationRequestValidator _validator =
        new(new FixedClock(new DateTimeOffset(Today, TimeOnly.MinValue, TimeSpan.Zero)));

    [Fact]
    public void AValidRequest_HasNoErrors()
    {
        _validator.TestValidate(AValidRequest()).ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void CustomerName_IsRequired(string name)
    {
        _validator.TestValidate(AValidRequest() with { CustomerName = name })
            .ShouldHaveValidationErrorFor(request => request.CustomerName);
    }

    [Fact]
    public void CustomerName_HasAMaximumLength()
    {
        _validator.TestValidate(AValidRequest() with { CustomerName = new string('a', 201) })
            .ShouldHaveValidationErrorFor(request => request.CustomerName);
    }

    [Fact]
    public void DateOfBirth_MustBeInThePast()
    {
        _validator.TestValidate(AValidRequest() with { DateOfBirth = Today.AddDays(1) })
            .ShouldHaveValidationErrorFor(request => request.DateOfBirth);
    }

    [Fact]
    public void DateOfBirth_TodayIsNotInThePast()
    {
        _validator.TestValidate(AValidRequest() with { DateOfBirth = Today })
            .ShouldHaveValidationErrorFor(request => request.DateOfBirth);
    }

    [Fact]
    public void Applicant_MustHaveTurnedEighteen()
    {
        DateOnly dayBeforeEighteenth = Today.AddYears(-18).AddDays(1);

        _validator.TestValidate(AValidRequest() with { DateOfBirth = dayBeforeEighteenth })
            .ShouldHaveValidationErrorFor(request => request.DateOfBirth);
    }

    [Fact]
    public void Applicant_IsAcceptedOnTheirEighteenthBirthday()
    {
        _validator.TestValidate(AValidRequest() with { DateOfBirth = Today.AddYears(-18) })
            .ShouldNotHaveValidationErrorFor(request => request.DateOfBirth);
    }

    [Fact]
    public void MonthlyIncome_CannotBeNegative()
    {
        _validator.TestValidate(AValidRequest() with { MonthlyIncome = -0.01m })
            .ShouldHaveValidationErrorFor(request => request.MonthlyIncome);
    }

    [Fact]
    public void MonthlyExpenses_CannotBeNegative()
    {
        _validator.TestValidate(AValidRequest() with { MonthlyExpenses = -0.01m })
            .ShouldHaveValidationErrorFor(request => request.MonthlyExpenses);
    }

    [Fact]
    public void ExpensesAboveIncome_IsAccepted()
    {
        // The whole point of the form. A deficit must not be rejected.
        CreateHardshipApplicationRequest request =
            AValidRequest() with { MonthlyIncome = 2500m, MonthlyExpenses = 4100m };

        _validator.TestValidate(request).ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void HardshipReason_IsOptional()
    {
        _validator.TestValidate(AValidRequest() with { HardshipReason = null })
            .ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void HardshipReason_HasAMaximumLength()
    {
        _validator.TestValidate(AValidRequest() with { HardshipReason = new string('a', 1001) })
            .ShouldHaveValidationErrorFor(request => request.HardshipReason);
    }

    private static CreateHardshipApplicationRequest AValidRequest() => new(
        CustomerName: "Alex Taylor",
        DateOfBirth: new DateOnly(1990, 4, 17),
        MonthlyIncome: 5000m,
        MonthlyExpenses: 3200m,
        HardshipReason: "Reduced hours at work");
}
