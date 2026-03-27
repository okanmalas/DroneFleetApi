using System.ComponentModel.DataAnnotations;

namespace DroneFleetApi.DTOs.Drone;

public class DroneDTO
{
    [Required(ErrorMessage = "İHA model adı boş bırakılamaz.")]
    [MaxLength(50, ErrorMessage = "Model adı 50 karakterden uzun olamaz.")]
    public string ModelName { get; set; }

    [Required]
    [MaxLength(15)]
    public string IpAddress { get; set; }

    [Range(10, 3000, ErrorMessage = "Uçuş süresi 10 ile 3000 dakika arasında olmalıdır.")]
    public int MaxFlightTimeMinutes { get; set; }

    public bool IsActive { get; set; }
}