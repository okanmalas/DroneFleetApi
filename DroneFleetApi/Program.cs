#region Imports
using DroneFleetApi.DbContext;
using DroneFleetApi.DTOs;
using DroneFleetApi.Entities;
using DroneFleetApi.Filters;
using Microsoft.EntityFrameworkCore;
#endregion

#region Configurations
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});
var app = builder.Build();
#endregion

#region Middlewares

/*app.Use(async (context, next) =>
{
    //todo maybe add middleware
    await next();
    //endtodo
});*/

#endregion

#region Endpoints

app.MapGet("/drones", async (AppDbContext context) =>
{
    return await context.Drones
        .Include(d => d.FlightLogs)
        .ToListAsync();
}); //returns all drones listed in the database

app.MapGet("/drones/{id}", async (int id, AppDbContext context) =>
{
    var drone = await context.Drones
        .Include(d => d.FlightLogs)
        .FirstOrDefaultAsync(d => d.Id == id);
    
    if (drone == null)
        return Results.NotFound(new { Mesaj = $"{id} numaralı İHA bulunamadı." });
    
    return Results.Ok(drone);
}); //returns a drone with the given id

app.MapGet("/drones/active-summary", async (AppDbContext context) =>
{
    var activeDroneList = await context.Drones
        .Where(d => d.IsActive == true)
        .Select(d => new 
        { 
            AracAdi = d.ModelName, 
            BaglantiAdresi = d.IpAddress 
        })
        .ToListAsync();

    return Results.Ok(activeDroneList);
}); //returns a summary of active drones

app.MapPost("/drones", async (AppDbContext context, DroneDTO dto) =>
{
    var newDrone = new Drone
    {
        ModelName = dto.ModelName,
        IpAddress = dto.IpAddress,
        MaxFlightTimeMinutes = dto.MaxFlightTimeMinutes,
        IsActive = true,
        IsDeleted = false
    };
    await context.Drones.AddAsync(newDrone);
    await context.SaveChangesAsync();
    return Results.Created($"/drones/{newDrone.Id}", newDrone);
}).AddEndpointFilter<ValidationFilter<DroneDTO>>(); //adds a drone to the database

app.MapPut("/drones/{id}", async (int id, AppDbContext context, DroneDTO dto) =>
{
    var mevcutDrone = await context.Drones.FindAsync(id);
    if (mevcutDrone == null)
        return Results.NotFound(new { Mesaj = $"{id} numaralı İHA bulunamadı." });
    mevcutDrone.ModelName = dto.ModelName;
    mevcutDrone.IpAddress = dto.IpAddress;
    mevcutDrone.MaxFlightTimeMinutes = dto.MaxFlightTimeMinutes;
    mevcutDrone.IsActive = dto.IsActive;
    await context.SaveChangesAsync();
    return Results.Ok(mevcutDrone);
}).AddEndpointFilter<ValidationFilter<DroneDTO>>(); //updates a drone with the given id

app.MapPost("/drones/{id}/flightlogs", async (int id,FlightLogDTO dto, AppDbContext context) =>
{
    var drone = await context.Drones.FindAsync(id);
    if (drone == null)
        return Results.NotFound(new { Mesaj = $"{id} numaralı İHA bulunamadı, uçuş kaydı eklenemez!" });
    var yeniLog = new FlightLog
    {
        DroneId = drone.Id,
        LogDate = DateTime.UtcNow,
        Description = dto.Description
    };
    await context.FlightLogs.AddAsync(yeniLog);
    await context.SaveChangesAsync();
    return Results.Ok("Flight Log Added to Drone: " + drone.Id);
}).AddEndpointFilter<ValidationFilter<FlightLogDTO>>(); //adds a flight log to selected drone

app.MapDelete("/drones/{id}", async (int id, AppDbContext context) =>
{
    var drone = await context.Drones.FindAsync(id);
    if (drone == null)
        return Results.NotFound();
    drone.IsDeleted = true;
    context.Drones.Update(drone);
    await context.SaveChangesAsync();
    return Results.Ok();
}); //removes a drone with the given id

#endregion

app.Run();