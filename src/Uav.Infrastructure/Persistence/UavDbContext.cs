using Microsoft.EntityFrameworkCore;
using Uav.Domain.Entities;

namespace Uav.Infrastructure.Persistence;

public class UavDbContext : DbContext
{
    public DbSet<Drone> Drones { get; set; }
    public DbSet<Mission> Missions { get; set; }

    public UavDbContext(DbContextOptions<UavDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(UavDbContext).Assembly);
    }
}