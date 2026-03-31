using AutoMapper;
using DroneFleetApi.DTOs.Drone;
using DroneFleetApi.DTOs.FlightLog;
using DroneFleetApi.Entities;

namespace DroneFleetApi.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // 1. Veritabanından -> Dışarıya (GET İşlemleri İçin)
        CreateMap<Drone, DroneResponseDTO>();
        CreateMap<FlightLog, FlightLogResponseDTO>();
        CreateMap<Drone, DroneSummaryDTO>();

        // 2. Dışarıdan -> Veritabanına (POST ve PUT İşlemleri İçin)
        CreateMap<DroneDTO, Drone>();
        CreateMap<FlightLogDTO, FlightLog>();
    }
}