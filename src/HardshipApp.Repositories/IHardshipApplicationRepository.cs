using HardshipApp.Models;

namespace HardshipApp.Repositories;

/// <summary>
/// Data access for hardship applications.
/// </summary>
/// <remarks>
/// Deliberately not a generic <c>IRepository&lt;T&gt;</c>. A generic repository over EF Core
/// mostly restates what <c>DbSet</c> already is, and it pushes callers back towards
/// <c>IQueryable</c> — at which point the abstraction has bought nothing. Named methods say
/// what the application actually does with the data.
/// </remarks>
public interface IHardshipApplicationRepository
{
    /// <summary>Every application, newest first.</summary>
    Task<IReadOnlyList<HardshipApplication>> ListAsync(CancellationToken cancellationToken = default);

    /// <summary>The application with this id, or <c>null</c> if there is none.</summary>
    Task<HardshipApplication?> FindAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Stores a new application.</summary>
    Task AddAsync(HardshipApplication application, CancellationToken cancellationToken = default);

    /// <summary>Persists changes to an existing application.</summary>
    Task UpdateAsync(HardshipApplication application, CancellationToken cancellationToken = default);
}
