namespace HardshipApp.Models;

/// <summary>
/// A customer's application for financial hardship assistance.
/// </summary>
public class HardshipApplication
{
    public Guid Id { get; set; }

    public required string CustomerName { get; set; }

    public DateOnly DateOfBirth { get; set; }

    /// <summary>Monthly income, in AUD.</summary>
    public decimal MonthlyIncome { get; set; }

    /// <summary>Monthly expenses, in AUD.</summary>
    public decimal MonthlyExpenses { get; set; }

    /// <summary>Why the customer is applying. Optional — the applicant may not want to say.</summary>
    public string? HardshipReason { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset? UpdatedAt { get; set; }

    /// <summary>
    /// What is left over each month, which is the number a hardship assessment turns on.
    /// </summary>
    /// <remarks>
    /// Negative is the expected case, not an error: an applicant whose expenses exceed their
    /// income is describing exactly the situation hardship assistance exists for. Nothing in
    /// this system rejects a deficit.
    /// <para>
    /// Derived rather than stored, so it cannot drift out of step with the two figures it
    /// comes from.
    /// </para>
    /// </remarks>
    public decimal MonthlySurplus => MonthlyIncome - MonthlyExpenses;

    /// <summary>Whether the applicant is spending more than they earn.</summary>
    public bool IsInDeficit => MonthlySurplus < 0;
}
