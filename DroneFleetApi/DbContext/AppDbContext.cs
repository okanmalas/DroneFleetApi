using DroneFleetApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace DroneFleetApi.DbContext;

public class AppDbContext : Microsoft.EntityFrameworkCore.DbContext
{
    public DbSet<Drone> Drones { get; set; }
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    
    
}