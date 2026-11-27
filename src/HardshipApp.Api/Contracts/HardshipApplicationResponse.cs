namespace HardshipApp.Api.Contracts;

public sealed record HardshipApplicationResponse(
    Guid Id,
    string CustomerName,
    DateOnly DateOfBirth,
    decimal MonthlyIncome,
    decimal MonthlyExpenses,
    string? HardshipReason,
    decimal MonthlySurplus,
    bool IsInDeficit,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt);
