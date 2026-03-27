using DroneFleetApi.DTOs.FlightLog;

namespace DroneFleetApi.DTOs.Drone;

public class DroneResponseDTO
{
    public int Id { get; set; }
    public string ModelName { get; set; }
    public string IpAddress { get; set; }
    public int MaxFlightTimeMinutes { get; set; }
    public bool IsActive { get; set; }
    public List<FlightLogResponseDTO> FlightLogs { get; set; } = new List<FlightLogResponseDTO>();
}