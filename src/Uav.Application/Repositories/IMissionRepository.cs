using Uav.Domain.Entities;

namespace Uav.Application.Repositories;

public interface IMissionRepository
{
    Task<Mission?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Mission>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Mission?> GetByIdWithDroneAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(Mission mission, CancellationToken cancellationToken = default);
    Task UpdateAsync(Mission mission, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}