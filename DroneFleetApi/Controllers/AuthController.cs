using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DroneFleetApi.DTOs.Login;

namespace DroneFleetApi.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;

    // appsettings.json dosyasındaki o gizli şifreleri okumak için IConfiguration'ı enjekte ediyoruz
    public AuthController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginDTO dto)
    {
        // 1. KİMLİK KONTROLÜ (Şimdilik veritabanı yok, statik kontrol yapıyoruz)
        if (dto.Username != "admin" || dto.Password != "123456")
        {
            return Unauthorized(new { Mesaj = "Kullanıcı adı veya şifre hatalı!" });
        }

        // 2. ŞİFRELER DOĞRUYSA BİLET ÜRETİMİ (JWT) BAŞLAR
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!);

        // Biletin içine koyacağımız kimlik bilgileri (Payload)
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, dto.Username),
            new Claim(ClaimTypes.Role, "Admin")
        };

        // Biletin özellikleri (Kim üretti, kime gidiyor, süresi ne zaman dolacak?)
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(5), // Bilet 5 dakika sonra kendini imha edecek
            Issuer = jwtSettings["Issuer"],
            Audience = jwtSettings["Audience"],
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(secretKey), 
                SecurityAlgorithms.HmacSha256Signature) // Biletin altına basılan mühür
        };

        // Bileti basan makine çalışıyor...
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        // Bileti şifreli bir metne çevirip kullanıcıya teslim ediyoruz
        return Ok(new { Token = tokenHandler.WriteToken(token) });
    }
}