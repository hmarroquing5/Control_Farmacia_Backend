using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ControlFarmacia.API.Data;
using ControlFarmacia.API.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace ControlFarmacia.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;

        public AuthController(ApplicationDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

[HttpPost("login")]
public async Task<IActionResult> Login([FromBody] LoginRequest request)
{
    // 1. CAMBIADO A SINTAXIS MYSQL: CALL con paréntesis y marcadores estándar {0} y {1}
    var user = (await _context.Usuarios
        .FromSqlRaw("CALL sp_ValidarUsuario({0}, {1})", request.Username, request.Password)
        .ToListAsync())
        .FirstOrDefault();

    if (user == null) return Unauthorized(new { message = "Credenciales inválidas" });

    // 2. CORREGIDO: Buscamos primero en el Environment (JWT_KEY) igual que en el Program.cs
    var jwtKey = Environment.GetEnvironmentVariable("JWT_KEY") ?? _config["JWT_KEY"] ?? _config["Jwt:Key"];
    
    if (string.IsNullOrEmpty(jwtKey))
    {
        return StatusCode(500, new { message = "Error interno: La clave JWT no está configurada en el servidor." });
    }

    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
    var token = new JwtSecurityToken(
        issuer: _config["Jwt:Issuer"],
        audience: _config["Jwt:Audience"],
        claims: new[] { new Claim(ClaimTypes.Name, user.Username) },
        expires: DateTime.Now.AddHours(8),
        signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
    );

    var userDto = new UsuarioDTO
    {
        Id = user.Id,
        Username = user.Username
    };

    return Ok(new 
    { 
        token = new JwtSecurityTokenHandler().WriteToken(token),
        usuario = userDto
    });
}
    }
}