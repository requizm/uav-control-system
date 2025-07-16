using Uav.Domain.Enums;
using Uav.Domain.ValueObjects;

namespace Uav.Domain.Entities;

public class Drone
{
    public required Guid Id { get; init; }
    public required string ModelName { get; set; }

    public GpsCoordinate CurrentPosition { get; set; }
    public double BatteryPercentage { get; set; }
    public DroneStatus Status { get; set; }

    public Mission? CurrentMission { get; set; }
    public Guid? CurrentMissionId { get; set; }
}