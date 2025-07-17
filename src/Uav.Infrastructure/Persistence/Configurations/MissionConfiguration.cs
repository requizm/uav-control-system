using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Uav.Domain.Entities;
using Uav.Domain.ValueObjects;

namespace Uav.Infrastructure.Persistence.Configurations;

public class MissionConfiguration : IEntityTypeConfiguration<Mission>
{
    public void Configure(EntityTypeBuilder<Mission> builder)
    {
        builder.HasKey(m => m.Id);

        builder.Property(m => m.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(m => m.Waypoints)
            .HasColumnType("jsonb");
    }
}