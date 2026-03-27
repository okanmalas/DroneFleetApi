using DroneFleetApi.DbContext;
using DroneFleetApi.DTOs.Drone;
using DroneFleetApi.DTOs.FlightLog;
using DroneFleetApi.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DroneFleetApi.Controllers;

[ApiController]
[Route("drones")]
public class DronesController : ControllerBase
{
    private readonly AppDbContext _context;
    public DronesController(AppDbContext context) { _context = context; }
    #region http resquest "/drones"
    
    // BÜTÜN İHALARI GETİR (GET /drones)
    [HttpGet]
    public async Task<IActionResult> GetAllDrones()
    {
        var droneListesi = await _context.Drones
            .Select(d => new DroneResponseDTO
            {
                Id = d.Id,
                ModelName = d.ModelName,
                IpAddress = d.IpAddress,
                MaxFlightTimeMinutes = d.MaxFlightTimeMinutes,
                IsActive = d.IsActive,
                FlightLogs = d.FlightLogs.Select(f => new FlightLogResponseDTO
                {
                    Id = f.Id,
                    LogDate = f.LogDate,
                    Description = f.Description,
                    DroneId = f.DroneId
                }).ToList()
            })
            .ToListAsync();

        return Ok(droneListesi);
    }
    
    // 1. TEK BİR İHA GETİR (GET /drones/{id})
    [HttpGet("{id}")] 
    public async Task<IActionResult> GetDroneById(int id)
    {
        var drone = await _context.Drones
            .Include(d => d.FlightLogs)
            .FirstOrDefaultAsync(d => d.Id == id);
        
        if (drone == null)
            return NotFound(new { Mesaj = $"{id} numaralı İHA bulunamadı." });

        var response = new DroneResponseDTO
        {
            Id = drone.Id,
            ModelName = drone.ModelName,
            IpAddress = drone.IpAddress,
            MaxFlightTimeMinutes = drone.MaxFlightTimeMinutes,
            IsActive = drone.IsActive,
            FlightLogs = drone.FlightLogs.Select(f => new FlightLogResponseDTO
            {
                Id = f.Id,
                LogDate = f.LogDate,
                Description = f.Description,
                DroneId = f.DroneId
            }).ToList()
        };
        
        return Ok(response);
    }

    // 2. YENİ İHA EKLE (POST /drones)
    [HttpPost]
    public async Task<IActionResult> CreateDrone([FromBody] DroneDTO dto) // [FromBody]: Veriyi URL'den değil, JSON gövdesinden al demek
    {
        var newDrone = new Drone
        {
            ModelName = dto.ModelName,
            IpAddress = dto.IpAddress,
            MaxFlightTimeMinutes = dto.MaxFlightTimeMinutes,
            IsActive = true,
            IsDeleted = false
        };
        
        await _context.Drones.AddAsync(newDrone);
        await _context.SaveChangesAsync();

        var response = new DroneResponseDTO
        {
            Id = newDrone.Id,
            ModelName = newDrone.ModelName,
            IpAddress = newDrone.IpAddress,
            MaxFlightTimeMinutes = newDrone.MaxFlightTimeMinutes,
            IsActive = newDrone.IsActive,
            FlightLogs = new List<FlightLogResponseDTO>()
        };
        
        // Minimal API'deki Results.Created() yerine bunu kullanıyoruz.
        // Bizi direkt olarak yukarıdaki GetDroneById metoduna yönlendirir.
        return CreatedAtAction(nameof(GetDroneById), new { id = newDrone.Id }, response); 
    }

    // 3. İHA GÜNCELLE (PUT /drones/{id})
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateDrone(int id, [FromBody] DroneDTO dto)
    {
        var mevcutDrone = await _context.Drones
            .Include(d => d.FlightLogs)
            .FirstOrDefaultAsync(d => d.Id == id);
        
        if (mevcutDrone == null)
            return NotFound(new { Mesaj = $"{id} numaralı İHA bulunamadı." });

        mevcutDrone.ModelName = dto.ModelName;
        mevcutDrone.IpAddress = dto.IpAddress;
        mevcutDrone.MaxFlightTimeMinutes = dto.MaxFlightTimeMinutes;
        mevcutDrone.IsActive = dto.IsActive;
        
        await _context.SaveChangesAsync();

        var response = new DroneResponseDTO
        {
            Id = mevcutDrone.Id,
            ModelName = mevcutDrone.ModelName,
            IpAddress = mevcutDrone.IpAddress,
            MaxFlightTimeMinutes = mevcutDrone.MaxFlightTimeMinutes,
            IsActive = mevcutDrone.IsActive,
            FlightLogs = mevcutDrone.FlightLogs.Select(f => new FlightLogResponseDTO
            {
                Id = f.Id,
                LogDate = f.LogDate,
                Description = f.Description,
                DroneId = f.DroneId
            }).ToList()
        };

        return Ok(response);
    }

    // 4. İHA SİL (DELETE /drones/{id})
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDrone(int id)
    {
        var drone = await _context.Drones.FindAsync(id);
        
        if (drone == null)
            return NotFound();

        drone.IsDeleted = true; // Gerçekten silmiyoruz, pasife çekiyoruz (Soft Delete)
        _context.Drones.Update(drone);
        await _context.SaveChangesAsync();
        
        // Sektör Standardı: Silme işlemi başarılıysa ekrana veri dönülmez, sadece 204 (No Content) durum kodu yollanır.
        return NoContent(); 
    }
    
    // 5. AKTİF İHA'LARIN ÖZETİ (GET /drones/active-summary)
    [HttpGet("active-summary")] 
    public async Task<IActionResult> GetActiveDronesSummary()
    {
        var activeDroneList = await _context.Drones
            .Where(d => d.IsActive == true)
            .Select(d => new DroneSummaryDTO
            { 
                ModelName = d.ModelName, 
                IpAddress = d.IpAddress 
            })
            .ToListAsync();

        return Ok(activeDroneList);
    }

    // 6. UÇUŞ KAYDI EKLEME (POST /drones/{id}/flightlogs)
    [HttpPost("{id}/flightlogs")]
    public async Task<IActionResult> AddFlightLog(int id, [FromBody] FlightLogDTO dto)
    {
        var drone = await _context.Drones.FindAsync(id);
        
        if (drone == null)
            return NotFound(new { Mesaj = $"{id} numaralı İHA bulunamadı, uçuş kaydı eklenemez!" });
        
        var yeniLog = new FlightLog
        {
            DroneId = drone.Id,
            LogDate = DateTime.UtcNow,
            Description = dto.Description
        };
        
        await _context.FlightLogs.AddAsync(yeniLog);
        await _context.SaveChangesAsync();

        var response = new FlightLogResponseDTO
        {
            Id = yeniLog.Id,
            LogDate = yeniLog.LogDate,
            Description = yeniLog.Description,
            DroneId = yeniLog.DroneId
        };
        
        return Ok(new { Mesaj = $"Flight Log Added to Drone: {drone.Id}", Data = response });
    }
    
    #endregion
}