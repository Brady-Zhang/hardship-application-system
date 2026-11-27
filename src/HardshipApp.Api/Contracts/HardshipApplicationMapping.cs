using HardshipApp.Models;

namespace HardshipApp.Api.Contracts;

internal static class HardshipApplicationMapping
{
    public static HardshipApplication ToEntity(this HardshipApplicationRequest request)
    {
        HardshipApplication application = new() { CustomerName = request.CustomerName.Trim() };
        request.ApplyTo(application);
        return application;
    }

    public static void ApplyTo(this HardshipApplicationRequest request, HardshipApplication application)
    {
        application.CustomerName = request.CustomerName.Trim();
        application.DateOfBirth = request.DateOfBirth;
        application.MonthlyIncome = request.MonthlyIncome;
        application.MonthlyExpenses = request.MonthlyExpenses;
        application.HardshipReason = string.IsNullOrWhiteSpace(request.HardshipReason)
            ? null
            : request.HardshipReason.Trim();
    }

    // The surplus is sent rather than left to the client to work out, so the figure on
    // screen is the one the server computed.
    public static HardshipApplicationResponse ToResponse(this HardshipApplication application) => new(
        application.Id,
        application.CustomerName,
        application.DateOfBirth,
        application.MonthlyIncome,
        application.MonthlyExpenses,
        application.HardshipReason,
        application.MonthlySurplus,
        application.IsInDeficit,
        application.CreatedAt,
        application.UpdatedAt);
}
