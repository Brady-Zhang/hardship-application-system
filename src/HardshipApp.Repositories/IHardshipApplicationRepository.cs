using HardshipApp.Models;

namespace HardshipApp.Repositories;

public interface IHardshipApplicationRepository
{
    /// <summary>Every application, newest first.</summary>
    Task<IReadOnlyList<HardshipApplication>> ListAsync(CancellationToken cancellationToken = default);

    Task<HardshipApplication?> FindAsync(Guid id, CancellationToken cancellationToken = default);

    Task AddAsync(HardshipApplication application, CancellationToken cancellationToken = default);

    Task UpdateAsync(HardshipApplication application, CancellationToken cancellationToken = default);
}
