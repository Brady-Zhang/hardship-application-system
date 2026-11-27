using HardshipApp.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace HardshipApp.IntegrationTests.Support;

/// <summary>
/// A real SQLite database in a temporary file, migrated and deleted with the test.
/// </summary>
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
        // A file rather than :memory:, which would be empty again on the next connection.
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

    /// <summary>A repository on its own context, so a read-back really goes to the database.</summary>
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
