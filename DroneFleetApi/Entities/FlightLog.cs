using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DroneFleetApi.Entities;

public class FlightLog
{
    public int Id { get; set; }
    public DateTime LogDate { get; set; } = DateTime.UtcNow;
    
    [MaxLength(500, ErrorMessage = "Açıklama 500 karakterden uzun olamaz.")]
    public string Description { get; set; }
    
    public int DroneId { get; set; } 
    
    // C#'ta kod yazarken o Drone'un tüm bilgilerine buradan ulaşabilmemizi sağlar
    [JsonIgnore]
    public Drone Drone { get; set; } 
}