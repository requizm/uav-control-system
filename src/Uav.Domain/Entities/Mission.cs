using Uav.Domain.ValueObjects;

namespace Uav.Domain.Entities;

public class Mission
{
    public required Guid Id { get; init; }
    public required string Name { get; set; }
    public bool IsCompleted { get; set; }
    public Guid? AssignedDroneId { get; set; }
    public Drone? AssignedDrone { get; set; }

    public required List<GpsCoordinate> Waypoints { get; set; }
}