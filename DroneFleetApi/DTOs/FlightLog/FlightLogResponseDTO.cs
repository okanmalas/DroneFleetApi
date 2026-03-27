namespace DroneFleetApi.DTOs.FlightLog;

public class FlightLogResponseDTO
{
    public int Id { get; set; }
    public DateTime LogDate { get; set; }
    public string Description { get; set; }
    public int DroneId { get; set; }
}
