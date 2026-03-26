namespace DroneFleetApi.Entities;

public class Drone
{
    public int Id { get; set; }
    public string ModelName { get; set; }
    public string IpAddress { get; set; }
    public int MaxFlightTimeMinutes { get; set; }
    public bool IsActive { get; set; }
    public bool IsDeleted { get; set; }
}