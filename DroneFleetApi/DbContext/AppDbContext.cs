using DroneFleetApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace DroneFleetApi.DbContext;

public class AppDbContext : Microsoft.EntityFrameworkCore.DbContext
{
    public DbSet<Drone> Drones { get; set; }
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Drones tablosuna sorgu atıldığında HER ZAMAN sadece silinmemiş olanları getir
        modelBuilder.Entity<Drone>().HasQueryFilter(d => !d.IsDeleted);
    }
    
}