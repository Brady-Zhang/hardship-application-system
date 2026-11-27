using System.Reflection;
using HardshipApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace HardshipApp.Repositories;

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

    // Here rather than in each caller, so no write path can forget.
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
                    entry.Property(application => application.CreatedAt).IsModified = false;
                    break;
            }
        }
    }
}
