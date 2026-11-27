using HardshipApp.Models;
using Microsoft.EntityFrameworkCore;

namespace HardshipApp.Repositories;

internal sealed class HardshipApplicationRepository(HardshipDbContext database)
    : IHardshipApplicationRepository
{
    public async Task<IReadOnlyList<HardshipApplication>> ListAsync(
        CancellationToken cancellationToken = default) =>
        await database.HardshipApplications
            // Nothing is written back from the list, so there is no reason to pay for
            // change tracking on every row.
            .AsNoTracking()
            // Id breaks ties: two applications recorded in the same instant would otherwise
            // come back in whatever order the engine felt like, and the order would differ
            // between calls.
            .OrderByDescending(application => application.CreatedAt)
            .ThenByDescending(application => application.Id)
            .ToListAsync(cancellationToken);

    public Task<HardshipApplication?> FindAsync(
        Guid id,
        CancellationToken cancellationToken = default) =>
        // Tracked on purpose: the caller of this one is about to change it.
        database.HardshipApplications
            .FirstOrDefaultAsync(application => application.Id == id, cancellationToken);

    public async Task AddAsync(
        HardshipApplication application,
        CancellationToken cancellationToken = default)
    {
        database.HardshipApplications.Add(application);
        await database.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(
        HardshipApplication application,
        CancellationToken cancellationToken = default)
    {
        // Works whether or not the instance is already tracked, so the caller does not have
        // to know which. CreatedAt is held back by the context either way.
        database.HardshipApplications.Update(application);
        await database.SaveChangesAsync(cancellationToken);
    }
}
