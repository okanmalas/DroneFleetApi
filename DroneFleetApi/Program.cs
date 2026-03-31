#region Imports

using System.Text;
using DroneFleetApi.DbContext;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

#endregion

#region Configuration

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(config => config.AddMaps(typeof(Program).Assembly));
builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"]!))
        };
    });

var app = builder.Build();

#endregion

#region Middlewares

// Swagger Middleware'ini Etkinleştir
if (app.Environment.IsDevelopment() || true)
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

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

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();