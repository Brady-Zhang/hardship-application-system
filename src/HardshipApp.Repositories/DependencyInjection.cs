using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace HardshipApp.Repositories;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(
        this IServiceCollection services,
        string connectionString)
    {
        // TryAdd so a test can register a fake clock first.
        services.TryAddSingleton(TimeProvider.System);

        services.AddDbContext<HardshipDbContext>(options => options.UseSqlite(connectionString));

        services.AddScoped<IHardshipApplicationRepository, HardshipApplicationRepository>();

        return services;
    }
}
