using HardshipApp.Models;

namespace HardshipApp.Services;

public interface IHardshipApplicationService
{
    Task<HardshipApplication> CreateAsync(
        HardshipApplication application,
        CancellationToken cancellationToken = default);

    Task<HardshipApplication?> FindAsync(Guid id, CancellationToken cancellationToken = default);
}
