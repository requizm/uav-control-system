using Uav.Domain.Entities;

namespace Uav.Application.Repositories;

public interface IDroneRepository
{
    Task<Drone?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Drone>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Drone?> GetByIdWithMissionAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(Drone drone, CancellationToken cancellationToken = default);
    Task UpdateAsync(Drone drone, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}