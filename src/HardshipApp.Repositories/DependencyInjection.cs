using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace HardshipApp.Repositories;

public static class DependencyInjection
{
    /// <summary>
    /// Registers the database context and the data access this layer owns.
    /// </summary>
    /// <remarks>
    /// Takes the connection string rather than <c>IConfiguration</c> so this layer has no
    /// opinion about where configuration comes from — the host decides that.
    /// </remarks>
    public static IServiceCollection AddPersistence(
        this IServiceCollection services,
        string connectionString)
    {
        services.TryAddSingleton(TimeProvider.System);

        services.AddDbContext<HardshipDbContext>(options => options.UseSqlite(connectionString));

        return services;
    }
}
