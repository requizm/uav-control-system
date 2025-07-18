using Microsoft.AspNetCore.Mvc;
using Uav.Application.Common;
using Uav.Application.Repositories;
using Uav.Domain.Entities;
using Uav.Domain.Enums;
using Uav.Domain.ValueObjects;

namespace Uav.Control.Api.Controllers;

public record CreateMissionDto(string Name, List<GpsCoordinate> Waypoints);

public record AssignMissionDto(Guid DroneId);

public record MissionDto(Guid Id, string Name, bool IsCompleted, Guid? AssignedDroneId, List<GpsCoordinate> Waypoints);

[ApiController]
[Route("api/[controller]")]
public class MissionsController : ControllerBase
{
    private readonly IMissionRepository _missionRepository;
    private readonly IDroneRepository _droneRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<MissionsController> _logger;

    public MissionsController(IMissionRepository missionRepository, IDroneRepository droneRepository,
        IUnitOfWork unitOfWork, ILogger<MissionsController> logger)
    {
        _missionRepository = missionRepository;
        _droneRepository = droneRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    // POST: /api/missions
    [HttpPost]
    public async Task<IActionResult> CreateMission([FromBody] CreateMissionDto createDto)
    {
        if (createDto.Waypoints is null || createDto.Waypoints.Count == 0)
        {
            return BadRequest("A mission must have at least one waypoint.");
        }

        var mission = new Mission
        {
            Id = Guid.NewGuid(),
            Name = createDto.Name,
            IsCompleted = false,
            Waypoints = createDto.Waypoints
        };

        await _missionRepository.AddAsync(mission);
        await _unitOfWork.SaveChangesAsync();
        _logger.LogInformation("New mission created with ID {MissionId}", mission.Id);

        var missionDto = new MissionDto(mission.Id, mission.Name, mission.IsCompleted, null, []);
        return CreatedAtAction(nameof(GetMissionById), new { id = mission.Id }, missionDto);
    }

    // GET: /api/missions/{id}
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetMissionById(Guid id)
    {
        var mission = await _missionRepository.GetByIdWithDroneAsync(id);

        if (mission is null)
        {
            return NotFound();
        }

        var missionDto = new MissionDto(mission.Id, mission.Name, mission.IsCompleted, mission.AssignedDroneId,
            mission.Waypoints);
        return Ok(missionDto);
    }

    // POST: /api/missions/{id}/assign
    [HttpPost("{id:guid}/assign")]
    public async Task<IActionResult> AssignMission(Guid id, [FromBody] AssignMissionDto assignDto)
    {
        _logger.LogInformation("Attempting to assign mission {MissionId} to drone {DroneId}", id, assignDto.DroneId);

        var drone = await _droneRepository.GetByIdWithMissionAsync(assignDto.DroneId);
        if (drone is null)
        {
            return NotFound($"Drone with ID {assignDto.DroneId} not found.");
        }

        if (drone.Status != DroneStatus.Grounded)
        {
            return BadRequest($"Drone {drone.Id} is not available. Current status: {drone.Status}");
        }

        if (drone.CurrentMissionId is not null)
        {
            return BadRequest($"Drone {drone.Id} is already assigned to another mission.");
        }

        var mission = await _missionRepository.GetByIdAsync(id);
        if (mission is null)
        {
            return NotFound($"Mission with ID {id} not found.");
        }

        if (mission.IsCompleted)
        {
            return BadRequest("Cannot assign a completed mission.");
        }

        drone.Status = DroneStatus.InFlight;
        drone.CurrentMissionId = mission.Id;
        mission.AssignedDroneId = drone.Id;

        await _droneRepository.UpdateAsync(drone);
        await _missionRepository.UpdateAsync(mission);

        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Successfully assigned mission {MissionId} to drone {DroneId}", id, assignDto.DroneId);
        return Ok($"Mission {id} assigned to Drone {assignDto.DroneId}.");
    }

    // POST: /api/missions/{id}/complete
    [HttpPost("{id:guid}/complete")]
    public async Task<IActionResult> CompleteMission(Guid id)
    {
        var mission = await _missionRepository.GetByIdWithDroneAsync(id);
        if (mission is null)
        {
            return NotFound($"Mission with ID {id} not found.");
        }

        mission.IsCompleted = true;

        if (mission.AssignedDrone is not null)
        {
            mission.AssignedDrone.Status = DroneStatus.Grounded;
            mission.AssignedDrone.CurrentMissionId = null;
            await _droneRepository.UpdateAsync(mission.AssignedDrone);
        }

        await _missionRepository.UpdateAsync(mission);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Mission {MissionId} has been marked as complete.", id);
        return Ok();
    }
}