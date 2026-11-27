using HardshipApp.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HardshipApp.Api;

internal static class MigrationExtensions
{
    // Convenient for a fresh clone, wrong for production — see the README.
    public static async Task ApplyMigrationsAsync(this WebApplication app)
    {
        await using AsyncServiceScope scope = app.Services.CreateAsyncScope();

        HardshipDbContext database = scope.ServiceProvider.GetRequiredService<HardshipDbContext>();
        await database.Database.MigrateAsync();
    }
}
