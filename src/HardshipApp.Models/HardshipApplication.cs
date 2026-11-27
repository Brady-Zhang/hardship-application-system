namespace HardshipApp.Models;

public class HardshipApplication
{
    public Guid Id { get; set; }

    public required string CustomerName { get; set; }

    public DateOnly DateOfBirth { get; set; }

    /// <summary>Monthly income in AUD.</summary>
    public decimal MonthlyIncome { get; set; }

    /// <summary>Monthly expenses in AUD.</summary>
    public decimal MonthlyExpenses { get; set; }

    public string? HardshipReason { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset? UpdatedAt { get; set; }

    // Negative is expected, not an error: a deficit is what hardship assistance is for.
    public decimal MonthlySurplus => MonthlyIncome - MonthlyExpenses;

    public bool IsInDeficit => MonthlySurplus < 0;
}
