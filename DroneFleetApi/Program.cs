#region Imports
using DroneFleetApi.DbContext;
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

builder.Services.AddControllers();
var app = builder.Build();

#endregion

#region Middlewares

app.Use(async (context, next) =>
{
    try
    {
        await next(); 
    }
    catch (Exception ex)
    {
        string logKlasoru = Path.Combine(Directory.GetCurrentDirectory(), "Logs");
        if (!Directory.Exists(logKlasoru))
            Directory.CreateDirectory(logKlasoru);
        string dosyaYolu = Path.Combine(logKlasoru, "error_logs.txt");
        string logMetni = $"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}] [HATA] URL: {context.Request.Path} | Mesaj: {ex.Message}{Environment.NewLine}";
        await File.AppendAllTextAsync(dosyaYolu, logMetni);
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json"; 
        var hataCevabi = new 
        { 
            Mesaj = "Sunucu tarafında beklenmeyen kritik bir hata oluştu.", 
            HataDetayi = ex.Message 
        };
        await context.Response.WriteAsJsonAsync(hataCevabi);
    }
}); // global error handling

#endregion

app.MapControllers();
app.Run();