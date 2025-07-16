namespace Uav.Domain.ValueObjects;

/// <summary>
/// Represents a geographical coordinate.
/// </summary>
public readonly record struct GpsCoordinate(double Latitude, double Longitude);