using AutoMapper;
using DroneFleetApi.DbContext;
using DroneFleetApi.DTOs.Drone;
using DroneFleetApi.DTOs.FlightLog;
using DroneFleetApi.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DroneFleetApi.Controllers;

[ApiController]
[Route("drones")]
public class DronesController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;
    public DronesController(AppDbContext context, IMapper mapper) 
    {
        _context = context;
        _mapper = mapper;
    }
    
    #region http resquest "/drones"
    
    // BÜTÜN İHALARI GETİR (Filtreleme + Sayfalama)
    [HttpGet]
    public async Task<IActionResult> GetAllDrones(
        [FromQuery] int page = 1, 
        [FromQuery] int pageSize = 10,
        [FromQuery] bool? isActive = null, 
        [FromQuery] string? search = null) 
    {
        var query = _context.Drones.Include(d => d.FlightLogs).AsQueryable();

        if (isActive.HasValue)
            query = query.Where(d => d.IsActive == isActive.Value);

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(d => d.ModelName.ToLower().Contains(search.ToLower())); // %search% araması yapar

        var drones = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(); 

        var droneListesi = _mapper.Map<List<DroneResponseDTO>>(drones); //ham liste (drones) -> DTO liste (droneListesi)

        return Ok(droneListesi);
    }

    // 1. TEK BİR İHA GETİR (GET /drones/{id})
    [HttpGet("{id}")] 
    public async Task<IActionResult> GetDroneById(
        int id)
    {
        var drone = await _context.Drones
            .Include(d => d.FlightLogs)
            .FirstOrDefaultAsync(d => d.Id == id);
        
        if (drone == null)
            return NotFound(new { Mesaj = $"{id} numaralı İHA bulunamadı." });

        var response = _mapper.Map<DroneResponseDTO>(drone);
        
        return Ok(response);
    }

    // 2. YENİ İHA EKLE (POST /drones)
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateDrone(
        [FromBody] DroneDTO dto) // [FromBody]: Veriyi URL'den değil, JSON gövdesinden al demek
    {
        var newDrone = _mapper.Map<Drone>(dto);
        await _context.Drones.AddAsync(newDrone);
        await _context.SaveChangesAsync();
        var response = _mapper.Map<DroneResponseDTO>(newDrone);
        
        return CreatedAtAction(nameof(GetDroneById), new { id = newDrone.Id }, response); 
    }

    // 3. İHA GÜNCELLE (PUT /drones/{id})
    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateDrone(
        int id, 
        [FromBody] DroneDTO dto)
    {
        var mevcutDrone = await _context.Drones
            .Include(d => d.FlightLogs)
            .FirstOrDefaultAsync(d => d.Id == id);
        
        if (mevcutDrone == null)
            return NotFound(new { Mesaj = $"{id} numaralı İHA bulunamadı." });

        mevcutDrone = _mapper.Map(dto, mevcutDrone);
        
        await _context.SaveChangesAsync();

        var response = _mapper.Map<DroneResponseDTO>(mevcutDrone);

        return Ok(response);
    }

    // 4. İHA SİL (DELETE /drones/{id})
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDrone(
        int id)
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

    // 5. UÇUŞ KAYDI EKLEME (POST /drones/{id}/flightlogs)
    [Authorize(Roles = "Admin")]
    [HttpPost("{id}/flightlogs")]
    public async Task<IActionResult> AddFlightLog(
        int id,
        [FromBody] FlightLogDTO dto)
    {
        var drone = await _context.Drones.FindAsync(id);
        
        if (drone == null)
            return NotFound(new { Mesaj = $"{id} numaralı İHA bulunamadı, uçuş kaydı eklenemez!" });
        
        var yeniLog = _mapper.Map<FlightLog>(dto);
        await _context.FlightLogs.AddAsync(yeniLog);
        await _context.SaveChangesAsync();
        var response = _mapper.Map<FlightLogResponseDTO>(yeniLog);
        
        return Ok(new { Mesaj = $"Flight Log Added to Drone: {drone.Id}", Data = response });
    }
    
    #endregion
}