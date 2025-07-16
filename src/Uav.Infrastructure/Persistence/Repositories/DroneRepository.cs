using Microsoft.EntityFrameworkCore;
using Uav.Application.Repositories;
using Uav.Domain.Entities;

namespace Uav.Infrastructure.Persistence.Repositories;

public class DroneRepository : IDroneRepository
{
    private readonly UavDbContext _context;

    public DroneRepository(UavDbContext context)
    {
        _context = context;
    }

    public async Task<Drone?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Drones.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<IEnumerable<Drone>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Drones.ToListAsync(cancellationToken);
    }
    
    public async Task<Drone?> GetByIdWithMissionAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Drones
            .Include(d => d.CurrentMission)
            .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
    }

    public async Task AddAsync(Drone drone, CancellationToken cancellationToken = default)
    {
        await _context.Drones.AddAsync(drone, cancellationToken);
    }

    public async Task UpdateAsync(Drone drone, CancellationToken cancellationToken = default)
    {
        _context.Drones.Update(drone);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var drone = await GetByIdAsync(id, cancellationToken);
        if (drone is not null)
        {
            _context.Drones.Remove(drone);
        }
    }
}