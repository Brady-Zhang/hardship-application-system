using HardshipApp.Models;

namespace HardshipApp.UnitTests.Models;

public class HardshipApplicationTests
{
    [Theory]
    [InlineData(5000, 3200, 1800)]
    [InlineData(3000, 3000, 0)]
    [InlineData(2500, 4100, -1600)]
    [InlineData(0, 1200, -1200)]
    public void MonthlySurplus_IsIncomeLessExpenses(decimal income, decimal expenses, decimal expected)
    {
        HardshipApplication application = AnApplication(income, expenses);

        Assert.Equal(expected, application.MonthlySurplus);
    }

    [Fact]
    public void MonthlySurplus_KeepsCents()
    {
        HardshipApplication application = AnApplication(income: 4321.55m, expenses: 1234.45m);

        Assert.Equal(3087.10m, application.MonthlySurplus);
    }

    [Theory]
    [InlineData(2500, 4100, true)]
    [InlineData(4100, 2500, false)]
    [InlineData(3000, 3000, false)]
    public void IsInDeficit_IsTrueOnlyWhenExpensesExceedIncome(
        decimal income, decimal expenses, bool expected)
    {
        HardshipApplication application = AnApplication(income, expenses);

        Assert.Equal(expected, application.IsInDeficit);
    }

    private static HardshipApplication AnApplication(decimal income, decimal expenses) => new()
    {
        CustomerName = "Alex Taylor",
        DateOfBirth = new DateOnly(1990, 4, 17),
        MonthlyIncome = income,
        MonthlyExpenses = expenses,
    };
}
