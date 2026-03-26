#region Imports
using DroneFleetApi.DbContext;
using Microsoft.EntityFrameworkCore;
#endregion

#region Configurations
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
var app = builder.Build();
#endregion



#region Endpoints

app.MapGet("/", () => "Hello World!");

#endregion



app.Run();