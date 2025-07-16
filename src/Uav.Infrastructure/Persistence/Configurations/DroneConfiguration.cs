using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Uav.Domain.Entities;

namespace Uav.Infrastructure.Persistence.Configurations;

public class DroneConfiguration : IEntityTypeConfiguration<Drone>
{
    public void Configure(EntityTypeBuilder<Drone> builder)
    {
        builder.HasKey(d => d.Id);

        builder.Property(d => d.ModelName)
            .IsRequired()
            .HasMaxLength(100);

        builder.ComplexProperty(d => d.CurrentPosition);
    }
}