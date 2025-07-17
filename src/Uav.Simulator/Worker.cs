using System.Net.Http.Json;
using Uav.Domain.Entities;
using Uav.Domain.Enums;

namespace Uav.Simulator;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IHttpClientFactory _httpClientFactory;

    public Worker(ILogger<Worker> logger, IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("UAV Simulator starting up.");

        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Simulator running at: {time}", DateTimeOffset.Now);

            var apiClient = _httpClientFactory.CreateClient("UavApi");

            try
            {
                // 1. Get all drones from the API
                var drones = await apiClient.GetFromJsonAsync<List<Drone>>("drones", stoppingToken);

                if (drones is null || drones.Count == 0)
                {
                    _logger.LogInformation("No drones found. Waiting...");
                    await Task.Delay(5000, stoppingToken); // Wait 5 seconds before checking again
                    continue;
                }

                // 2. Filter for drones that are currently on a mission
                var inflightDrones = drones.Where(d => d.Status == DroneStatus.InFlight && d.CurrentMissionId.HasValue)
                    .ToList();

                if (inflightDrones.Any())
                {
                    _logger.LogInformation("Found {Count} in-flight drones to simulate.", inflightDrones.Count);

                    // 3. Create a simulation task for each drone and run them in parallel
                    var simulationTasks =
                        inflightDrones.Select(drone => SimulateDroneFlight(apiClient, drone, stoppingToken));
                    await Task.WhenAll(simulationTasks);
                }
                else
                {
                    _logger.LogInformation("No drones are currently in-flight.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during simulation cycle.");
            }

            // Wait for 10 seconds before starting the next simulation cycle.
            await Task.Delay(10000, stoppingToken);
        }
    }

    private async Task SimulateDroneFlight(HttpClient apiClient, Drone drone, CancellationToken stoppingToken)
    {
        _logger.LogInformation("Simulating flight for Drone {DroneId}", drone.Id);

        // Fetch the mission details
        var mission = await apiClient.GetFromJsonAsync<Mission>($"missions/{drone.CurrentMissionId}", stoppingToken);
        if (mission is null || mission.Waypoints.Count == 0)
        {
            _logger.LogWarning("Drone {DroneId} has an invalid or empty mission. Grounding it.", drone.Id);
            // You would call an API endpoint here to ground the drone
            return;
        }

        // Simple simulation: "move" to the next waypoint.
        // A real simulation would interpolate between points.
        // We'll just pick the final waypoint for simplicity.
        var targetWaypoint = mission.Waypoints.Last();

        _logger.LogInformation("Drone {DroneId} moving towards waypoint ({Lat}, {Lon})",
            drone.Id, targetWaypoint.Latitude, targetWaypoint.Longitude);

        // Simulate flight time
        await Task.Delay(5000, stoppingToken);

        _logger.LogInformation("Updating drone {DroneId} position via PATCH.", drone.Id);

        // Create the DTO with only the property we want to change.
        var patchDto = new { currentPosition = targetWaypoint };

        // Send a standard PATCH request. No special content type needed.
        var response = await apiClient.PatchAsJsonAsync($"drones/{drone.Id}", patchDto, stoppingToken);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Failed to update drone {DroneId}. Status: {StatusCode}", drone.Id, response.StatusCode);
            return; // Stop simulating for this drone if the update fails
        }

        // Check if mission is complete
        // We compare the target waypoint with the new position we just set.
        if (targetWaypoint.Equals(drone.CurrentPosition)) // This logic needs improvement, see below
        {
            _logger.LogInformation("Drone {DroneId} has reached its final waypoint. Completing mission.", drone.Id);

            // Call the endpoint to mark the mission as complete
            var completeResponse =
                await apiClient.PostAsync($"missions/{drone.CurrentMissionId}/complete", null, stoppingToken);
            if (completeResponse.IsSuccessStatusCode)
            {
                _logger.LogInformation("Successfully marked mission {MissionId} as complete.", drone.CurrentMissionId);
            }
            else
            {
                _logger.LogError("Failed to complete mission {MissionId}. Status: {StatusCode}", drone.CurrentMissionId,
                    completeResponse.StatusCode);
            }
        }
    }
}