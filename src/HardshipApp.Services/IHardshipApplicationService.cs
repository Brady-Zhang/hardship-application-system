using HardshipApp.Models;

namespace HardshipApp.Services;

public interface IHardshipApplicationService
{
    Task<IReadOnlyList<HardshipApplication>> ListAsync(CancellationToken cancellationToken = default);

    Task<HardshipApplication> CreateAsync(
        HardshipApplication application,
        CancellationToken cancellationToken = default);

    Task<HardshipApplication?> FindAsync(Guid id, CancellationToken cancellationToken = default);

    Task<HardshipApplication> UpdateAsync(
        HardshipApplication application,
        CancellationToken cancellationToken = default);
}
