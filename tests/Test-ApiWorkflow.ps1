# ===================================================================
# UAV Control System - API End-to-End Test Script
#
# This script performs a full workflow:
# 1. Creates a new Drone.
# 2. Creates a new Mission.
# 3. Assigns the Mission to the Drone.
# 4. Verifies the state of both entities.
# 5. Cleans up by deleting the created resources.
# ===================================================================

# --- Configuration ---
$baseUrl = "http://localhost:5008/api" # <-- IMPORTANT: Update port if necessary
$headers = @{"Content-Type"="application/json"}
$ErrorActionPreference = "Stop" # Stop the script immediately if any command fails

# --- Helper Function for Clean Output ---
function Write-Header {
    param([string]$Title)
    Write-Host "--------------------------------------------"
    Write-Host "  $Title"
    Write-Host "--------------------------------------------"
}

# --- Main Script Workflow ---
try {
    # 1. CREATE A NEW DRONE
    Write-Header "STEP 1: Creating a new Drone"
    $droneBody = @{
        modelName = "Global Hawk RQ-4"
    } | ConvertTo-Json
    
    $drone = Invoke-RestMethod -Uri "$baseUrl/drones" -Method Post -Headers $headers -Body $droneBody
    Write-Host "✅ SUCCESS: Drone created with ID: $($drone.id)"
    Write-Host ""
    
    
    # 2. CREATE A NEW MISSION
    Write-Header "STEP 2: Creating a new Mission"
    $missionBody = @{
        name      = "High-Altitude Reconnaissance"
        waypoints = @(
            @{ latitude = 34.639; longitude = -118.174 },
            @{ latitude = 34.645; longitude = -118.165 }
        )
    } | ConvertTo-Json
    
    $mission = Invoke-RestMethod -Uri "$baseUrl/missions" -Method Post -Headers $headers -Body $missionBody
    Write-Host "✅ SUCCESS: Mission created with ID: $($mission.id)"
    Write-Host ""
    
    
    # 3. ASSIGN THE MISSION TO THE DRONE
    Write-Header "STEP 3: Assigning Mission to Drone"
    $assignmentBody = @{
        droneId = $drone.id
    } | ConvertTo-Json
    
    $assignmentUrl = "$baseUrl/missions/$($mission.id)/assign"
    Invoke-RestMethod -Uri $assignmentUrl -Method Post -Headers $headers -Body $assignmentBody
    Write-Host "✅ SUCCESS: Mission '$($mission.name)' assigned to Drone '$($drone.modelName)'."
    Write-Host ""
    
    
    # 4. VERIFICATION
    Write-Header "STEP 4: Verifying state"
    
    # Verify Drone
    $updatedDrone = Invoke-RestMethod -Uri "$baseUrl/drones/$($drone.id)" -Method Get
    if ($updatedDrone.status -eq 1 -and $updatedDrone.CurrentMissionId -eq $mission.id) {
        Write-Host "✅ Drone Verification PASSED. Status is 'InFlight(1)' and Mission ID is correct."
    } else {
        Write-Error "❌ Drone Verification FAILED."
    }

    # Verify Mission
    $updatedMission = Invoke-RestMethod -Uri "$baseUrl/missions/$($mission.id)" -Method Get
    if ($updatedMission.assignedDroneId -eq $drone.id) {
        Write-Host "✅ Mission Verification PASSED. Assigned Drone ID is correct."
    } else {
        Write-Error "❌ Mission Verification FAILED."
    }
    Write-Host ""

}
catch {
    Write-Host "❌ An error occurred during the workflow:"
    Write-Host $_.Exception.Message
}
finally {
    # 5. CLEANUP
    Write-Header "STEP 5: Cleanup"
    
    # Always attempt to delete the created resources, even if the script failed midway.
    # The -ErrorAction SilentlyContinue prevents errors if the resource was never created.
    
#     if ($mission) {
#         try {
#             Invoke-RestMethod -Uri "$baseUrl/missions/$($mission.id)" -Method Delete -ErrorAction SilentlyContinue
#             Write-Host "✅ Mission $($mission.id) cleaned up."
#         } catch {}
#     }
# 
#     if ($drone) {
#         try {
#             Invoke-RestMethod -Uri "$baseUrl/drones/$($drone.id)" -Method Delete -ErrorAction SilentlyContinue
#             Write-Host "✅ Drone $($drone.id) cleaned up."
#         } catch {}
#     }
    
    Write-Host "✅ Cleanup complete."
}