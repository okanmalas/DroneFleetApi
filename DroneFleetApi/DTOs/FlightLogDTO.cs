using System.ComponentModel.DataAnnotations;

namespace DroneFleetApi.DTOs;

public class FlightLogDTO
{
    [MaxLength(500, ErrorMessage = "Açıklama 500 karakterden uzun olamaz.")]
    public string Description { get; set; }
}