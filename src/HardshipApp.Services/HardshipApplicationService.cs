using HardshipApp.Models;
using HardshipApp.Repositories;

namespace HardshipApp.Services;

internal sealed class HardshipApplicationService(IHardshipApplicationRepository repository)
    : IHardshipApplicationService
{
    public Task<IReadOnlyList<HardshipApplication>> ListAsync(CancellationToken cancellationToken = default) =>
        repository.ListAsync(cancellationToken);

    public async Task<HardshipApplication> CreateAsync(
        HardshipApplication application,
        CancellationToken cancellationToken = default)
    {
        // The id is assigned here rather than by the caller, so a client cannot choose it.
        application.Id = Guid.NewGuid();

        await repository.AddAsync(application, cancellationToken);

        return application;
    }

    public Task<HardshipApplication?> FindAsync(Guid id, CancellationToken cancellationToken = default) =>
        repository.FindAsync(id, cancellationToken);

    public async Task<HardshipApplication> UpdateAsync(
        HardshipApplication application,
        CancellationToken cancellationToken = default)
    {
        await repository.UpdateAsync(application, cancellationToken);

        return application;
    }
}
