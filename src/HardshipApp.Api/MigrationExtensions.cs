using HardshipApp.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HardshipApp.Api;

internal static class MigrationExtensions
{
    /// <summary>
    /// Brings the database up to date at start-up, so a fresh clone runs with one command.
    /// </summary>
    /// <remarks>
    /// Convenient here and wrong in production: several instances starting at once would race,
    /// and a failed migration would take the application down with it rather than failing the
    /// deployment. In a real deployment this runs as its own step before the new version is
    /// allowed to serve traffic.
    /// </remarks>
    public static async Task ApplyMigrationsAsync(this WebApplication app)
    {
        await using AsyncServiceScope scope = app.Services.CreateAsyncScope();

        HardshipDbContext database = scope.ServiceProvider.GetRequiredService<HardshipDbContext>();
        await database.Database.MigrateAsync();
    }
}
