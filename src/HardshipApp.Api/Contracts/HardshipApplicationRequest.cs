namespace HardshipApp.Api.Contracts;

public sealed record HardshipApplicationRequest(
    string CustomerName,
    DateOnly DateOfBirth,
    decimal MonthlyIncome,
    decimal MonthlyExpenses,
    string? HardshipReason);
