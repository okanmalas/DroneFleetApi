using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DroneFleetApi.Entities;

public class Drone
{
    public int Id { get; set; }
    
    [Required(ErrorMessage = "İHA model adı boş bırakılamaz.")]
    [MaxLength(50, ErrorMessage = "Model adı 50 karakterden uzun olamaz.")] 
    public string ModelName { get; set; }
    
    [Required]
    [MaxLength(15)]
    public string IpAddress { get; set; }
    
    [Range(10, 3000, ErrorMessage = "Uçuş süresi 10 ile 3000 dakika arasında olmalıdır.")]
    public int MaxFlightTimeMinutes { get; set; }
    
    public bool IsActive { get; set; }
    
    [JsonIgnore]
    public bool IsDeleted { get; set; }
    
    public List<FlightLog> FlightLogs { get; set; } = new List<FlightLog>();
}