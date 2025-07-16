using Microsoft.EntityFrameworkCore;
using Uav.Application.Repositories;
using Uav.Domain.Entities;

namespace Uav.Infrastructure.Persistence.Repositories;

public class MissionRepository : IMissionRepository
{
    private readonly UavDbContext _context;

    public MissionRepository(UavDbContext context)
    {
        _context = context;
    }

    public async Task<Mission?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Missions.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<IEnumerable<Mission>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Missions.ToListAsync(cancellationToken);
    }
    
    public async Task<Mission?> GetByIdWithDroneAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Missions
            .Include(m => m.AssignedDrone) // This tells EF Core to load the related Drone entity
            .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
    }

    public async Task AddAsync(Mission mission, CancellationToken cancellationToken = default)
    {
        await _context.Missions.AddAsync(mission, cancellationToken);
    }

    public async Task UpdateAsync(Mission mission, CancellationToken cancellationToken = default)
    {
        _context.Missions.Update(mission);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var mission = await GetByIdAsync(id, cancellationToken);
        if (mission is not null)
        {
            _context.Missions.Remove(mission);
        }
    }
}