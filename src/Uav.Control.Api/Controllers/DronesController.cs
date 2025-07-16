using Microsoft.AspNetCore.Mvc;
using Uav.Application.Common;
using Uav.Application.Repositories;
using Uav.Domain.Entities;
using Uav.Domain.Enums;
using Uav.Domain.ValueObjects;

namespace Uav.Control.Api.Controllers;

[ApiController]
[Route("api/[controller]")] // /api/drones
public class DronesController : ControllerBase
{
    private readonly IDroneRepository _droneRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DronesController> _logger;

    // The repository and logger are injected via the constructor by the DI container.
    public DronesController(IDroneRepository droneRepository, IUnitOfWork unitOfWork, ILogger<DronesController> logger)
    {
        _droneRepository = droneRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    // GET: /api/drones
    [HttpGet]
    public async Task<IActionResult> GetAllDrones()
    {
        var drones = await _droneRepository.GetAllAsync();
        return Ok(drones);
    }

    // GET: /api/drones/{id}
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetDroneById(Guid id)
    {
        var drone = await _droneRepository.GetByIdAsync(id);

        if (drone is null)
        {
            return NotFound($"Drone with ID {id} not found.");
        }

        return Ok(drone);
    }

    // POST: /api/drones
    public record CreateDroneDto(string ModelName);

    [HttpPost]
    public async Task<IActionResult> CreateDrone([FromBody] CreateDroneDto createDto)
    {
        if (string.IsNullOrWhiteSpace(createDto.ModelName))
        {
            return BadRequest("ModelName cannot be empty.");
        }

        var newDrone = new Drone
        {
            Id = Guid.NewGuid(),
            ModelName = createDto.ModelName,
            BatteryPercentage = 100.0, // Default to a full battery
            Status = DroneStatus.Grounded,
            CurrentPosition = new GpsCoordinate(0, 0) // Default starting position
        };

        await _droneRepository.AddAsync(newDrone);
        await _unitOfWork.SaveChangesAsync();
        _logger.LogInformation("New drone created with ID: {DroneId}", newDrone.Id);

        // Return a 201 Created response with a link to the new resource.
        return CreatedAtAction(nameof(GetDroneById), new { id = newDrone.Id }, newDrone);
    }

    // DELETE: /api/drones/{id}
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteDrone(Guid id)
    {
        var drone = await _droneRepository.GetByIdAsync(id);
        if (drone is null)
        {
            return NotFound();
        }

        await _droneRepository.DeleteAsync(id);
        await _unitOfWork.SaveChangesAsync();

        // Return 204 No Content, which is standard for a successful DELETE.
        return NoContent();
    }
}