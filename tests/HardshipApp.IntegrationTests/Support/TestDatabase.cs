using HardshipApp.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace HardshipApp.IntegrationTests.Support;

/// <summary>
/// A real SQLite database in a temporary file, migrated and thrown away with the test.
/// </summary>
/// <remarks>
/// A file rather than <c>:memory:</c>: EF Core opens and closes connections as it pleases, and
/// an in-memory SQLite database belongs to its connection — it would be empty again by the
/// next query.
/// <para>
/// Built through <see cref="DependencyInjection.AddPersistence"/> rather than by constructing
/// a context by hand, so these tests also cover the registration the application depends on.
/// The clock is registered first, which is what <c>TryAddSingleton</c> inside
/// <c>AddPersistence</c> is there to allow.
/// </para>
/// </remarks>
internal sealed class TestDatabase : IAsyncDisposable
{
    private readonly string _path;
    private readonly ServiceProvider _provider;
    private readonly List<IServiceScope> _scopes = [];

    private TestDatabase(string path, ServiceProvider provider, FixedClock clock)
    {
        _path = path;
        _provider = provider;
        Clock = clock;
    }

    public FixedClock Clock { get; }

    public static async Task<TestDatabase> CreateAsync(DateTimeOffset now)
    {
        string path = Path.Combine(Path.GetTempPath(), $"hardship-tests-{Guid.NewGuid():N}.db");
        FixedClock clock = new(now);

        ServiceCollection services = new();
        services.AddSingleton<TimeProvider>(clock);
        services.AddPersistence($"Data Source={path}");

        ServiceProvider provider = services.BuildServiceProvider();

        await using (AsyncServiceScope scope = provider.CreateAsyncScope())
        {
            HardshipDbContext database = scope.ServiceProvider.GetRequiredService<HardshipDbContext>();
            await database.Database.MigrateAsync();
        }

        return new TestDatabase(path, provider, clock);
    }

    /// <summary>
    /// A repository backed by its own context, so reading back something written earlier
    /// really does go to the database rather than to a change tracker that still holds it.
    /// </summary>
    public IHardshipApplicationRepository NewRepository()
    {
        IServiceScope scope = _provider.CreateScope();
        _scopes.Add(scope);
        return scope.ServiceProvider.GetRequiredService<IHardshipApplicationRepository>();
    }

    public async ValueTask DisposeAsync()
    {
        foreach (IServiceScope scope in _scopes)
        {
            scope.Dispose();
        }

        await _provider.DisposeAsync();

        File.Delete(_path);
    }
}
