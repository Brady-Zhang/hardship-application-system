using System.Reflection;
using HardshipApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace HardshipApp.Repositories;

/// <summary>
/// The application's database context.
/// </summary>
/// <remarks>
/// Named for the domain rather than <c>ApplicationDbContext</c>: in this system an
/// "application" is a hardship application, and a context called <c>ApplicationDbContext</c>
/// would read as though it belonged to one.
/// </remarks>
public class HardshipDbContext(DbContextOptions<HardshipDbContext> options, TimeProvider clock)
    : DbContext(options)
{
    public DbSet<HardshipApplication> HardshipApplications => Set<HardshipApplication>();

    public override Task<int> SaveChangesAsync(
        bool acceptAllChangesOnSuccess,
        CancellationToken cancellationToken = default)
    {
        StampTimestamps();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        StampTimestamps();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }

    /// <summary>
    /// Sets <c>CreatedAt</c> and <c>UpdatedAt</c> on the way out.
    /// </summary>
    /// <remarks>
    /// Here rather than in each caller: the second write path is the one that forgets, and a
    /// row with no <c>CreatedAt</c> cannot be ordered or audited after the fact. The clock is
    /// injected so a test can assert on an exact instant instead of a tolerance.
    /// </remarks>
    private void StampTimestamps()
    {
        DateTimeOffset now = clock.GetUtcNow();

        foreach (EntityEntry<HardshipApplication> entry in ChangeTracker.Entries<HardshipApplication>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = now;
                    entry.Entity.UpdatedAt = null;
                    break;
                case EntityState.Modified:
                    entry.Entity.UpdatedAt = now;
                    // Nobody gets to rewrite when a record was created.
                    entry.Property(application => application.CreatedAt).IsModified = false;
                    break;
            }
        }
    }
}
