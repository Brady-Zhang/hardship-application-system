using HardshipApp.IntegrationTests.Support;
using HardshipApp.Models;
using HardshipApp.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HardshipApp.IntegrationTests.Persistence;

public class HardshipApplicationRepositoryTests
{
    private static readonly DateTimeOffset Now = new(2026, 11, 27, 9, 30, 0, TimeSpan.Zero);

    [Fact]
    public async Task AddAsync_RoundTripsEveryField()
    {
        await using TestDatabase database = await TestDatabase.CreateAsync(Now);
        HardshipApplication application = AnApplication();

        await database.NewRepository().AddAsync(application);
        HardshipApplication? stored = await database.NewRepository().FindAsync(application.Id);

        Assert.NotNull(stored);
        Assert.Equal("Alex Taylor", stored.CustomerName);
        Assert.Equal(new DateOnly(1990, 4, 17), stored.DateOfBirth);
        Assert.Equal(4321.55m, stored.MonthlyIncome);
        Assert.Equal(1234.45m, stored.MonthlyExpenses);
        Assert.Equal("Reduced hours at work", stored.HardshipReason);
    }

    [Fact]
    public async Task AddAsync_KeepsCentsExactly()
    {
        await using TestDatabase database = await TestDatabase.CreateAsync(Now);
        HardshipApplication application = AnApplication();

        await database.NewRepository().AddAsync(application);
        HardshipApplication? stored = await database.NewRepository().FindAsync(application.Id);

        Assert.Equal(3087.10m, stored!.MonthlySurplus);
    }

    [Fact]
    public async Task AddAsync_AcceptsADeficit()
    {
        await using TestDatabase database = await TestDatabase.CreateAsync(Now);
        HardshipApplication application = AnApplication();
        application.MonthlyIncome = 2500m;
        application.MonthlyExpenses = 4100m;

        await database.NewRepository().AddAsync(application);
        HardshipApplication? stored = await database.NewRepository().FindAsync(application.Id);

        Assert.Equal(-1600m, stored!.MonthlySurplus);
        Assert.True(stored.IsInDeficit);
    }

    [Fact]
    public async Task AddAsync_StampsCreatedAtAndLeavesUpdatedAtUnset()
    {
        await using TestDatabase database = await TestDatabase.CreateAsync(Now);
        HardshipApplication application = AnApplication();

        await database.NewRepository().AddAsync(application);
        HardshipApplication? stored = await database.NewRepository().FindAsync(application.Id);

        Assert.Equal(Now, stored!.CreatedAt);
        Assert.Null(stored.UpdatedAt);
    }

    [Fact]
    public async Task UpdateAsync_StampsUpdatedAtAndLeavesCreatedAtAlone()
    {
        await using TestDatabase database = await TestDatabase.CreateAsync(Now);
        HardshipApplication application = AnApplication();
        await database.NewRepository().AddAsync(application);

        DateTimeOffset later = Now.AddDays(3);
        database.Clock.Now = later;

        IHardshipApplicationRepository repository = database.NewRepository();
        HardshipApplication? toEdit = await repository.FindAsync(application.Id);
        toEdit!.MonthlyExpenses = 2000m;
        await repository.UpdateAsync(toEdit);

        HardshipApplication? stored = await database.NewRepository().FindAsync(application.Id);
        Assert.Equal(2000m, stored!.MonthlyExpenses);
        Assert.Equal(later, stored.UpdatedAt);
        Assert.Equal(Now, stored.CreatedAt);
    }

    [Fact]
    public async Task ListAsync_ReturnsNewestFirst()
    {
        await using TestDatabase database = await TestDatabase.CreateAsync(Now);

        HardshipApplication first = AnApplication();
        first.CustomerName = "First";
        await database.NewRepository().AddAsync(first);

        database.Clock.Now = Now.AddHours(1);
        HardshipApplication second = AnApplication();
        second.CustomerName = "Second";
        await database.NewRepository().AddAsync(second);

        IReadOnlyList<HardshipApplication> all = await database.NewRepository().ListAsync();

        Assert.Equal(["Second", "First"], all.Select(application => application.CustomerName));
    }

    [Fact]
    public async Task AddAsync_WithNegativeIncome_IsRefusedByTheDatabase()
    {
        await using TestDatabase database = await TestDatabase.CreateAsync(Now);
        HardshipApplication application = AnApplication();
        application.MonthlyIncome = -1m;

        // The check constraint is the backstop for a write path that skips validation.
        await Assert.ThrowsAsync<DbUpdateException>(
            () => database.NewRepository().AddAsync(application));
    }

    private static HardshipApplication AnApplication() => new()
    {
        Id = Guid.NewGuid(),
        CustomerName = "Alex Taylor",
        DateOfBirth = new DateOnly(1990, 4, 17),
        MonthlyIncome = 4321.55m,
        MonthlyExpenses = 1234.45m,
        HardshipReason = "Reduced hours at work",
    };
}
