using Uav.Application.Common;

namespace Uav.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly UavDbContext _context;

    public UnitOfWork(UavDbContext context)
    {
        _context = context;
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }
}