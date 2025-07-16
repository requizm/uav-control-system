namespace Uav.Application.Common;

public interface IUnitOfWork
{
    // Save all changes made in a transaction.
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}