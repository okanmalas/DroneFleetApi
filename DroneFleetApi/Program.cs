#region Imports
using DroneFleetApi.DbContext;
using DroneFleetApi.Entities;
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

app.MapPost("/drones", async (AppDbContext context, Drone drone) =>
{
    await context.Drones.AddAsync(drone);
    drone.IsDeleted = false;
    await context.SaveChangesAsync();
    return Results.Created($"/drones/{drone.Id}", drone);
}); //adds a drone to the database

app.MapPut("/drones/{id}", async (int id, AppDbContext context, Drone guncelDrone) =>
{
    var mevcutDrone = await context.Drones.FindAsync(id);
    if (mevcutDrone == null)
        return Results.NotFound(new { Mesaj = $"{id} numaralı İHA bulunamadı." });
    mevcutDrone.ModelName = guncelDrone.ModelName;
    mevcutDrone.IpAddress = guncelDrone.IpAddress;
    mevcutDrone.MaxFlightTimeMinutes = guncelDrone.MaxFlightTimeMinutes;
    mevcutDrone.IsActive = guncelDrone.IsActive;
    await context.SaveChangesAsync();
    return Results.Ok(mevcutDrone);
}); //updates a drone with the given id

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

app.MapPost("/drones/{id}/flightlogs", async (int id,FlightLog yeniLog, AppDbContext context) =>
{
    var drone = await context.Drones.FindAsync(id);
    if (drone == null)
        return Results.NotFound(new { Mesaj = $"{id} numaralı İHA bulunamadı, uçuş kaydı eklenemez!" });
    yeniLog.DroneId = drone.Id;
    yeniLog.LogDate = DateTime.UtcNow;
    await context.FlightLogs.AddAsync(yeniLog);
    await context.SaveChangesAsync();
    return Results.Ok("Flight Log Added to Drone: " + drone.Id);
}); //adds a flight log to selected drone

#endregion

app.Run();