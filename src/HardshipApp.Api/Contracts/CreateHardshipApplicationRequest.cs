namespace HardshipApp.Api.Contracts;

public sealed record CreateHardshipApplicationRequest(
    string CustomerName,
    DateOnly DateOfBirth,
    decimal MonthlyIncome,
    decimal MonthlyExpenses,
    string? HardshipReason);
