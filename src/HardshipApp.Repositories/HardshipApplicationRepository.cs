using HardshipApp.Models;
using Microsoft.EntityFrameworkCore;

namespace HardshipApp.Repositories;

internal sealed class HardshipApplicationRepository(HardshipDbContext database)
    : IHardshipApplicationRepository
{
    public async Task<IReadOnlyList<HardshipApplication>> ListAsync(
        CancellationToken cancellationToken = default) =>
        await database.HardshipApplications
            .AsNoTracking()
            // Id breaks ties so paging and repeat calls stay in the same order.
            .OrderByDescending(application => application.CreatedAt)
            .ThenByDescending(application => application.Id)
            .ToListAsync(cancellationToken);

    public Task<HardshipApplication?> FindAsync(
        Guid id,
        CancellationToken cancellationToken = default) =>
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
        database.HardshipApplications.Update(application);
        await database.SaveChangesAsync(cancellationToken);
    }
}
