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
    var user = (await _context.Usuarios
        .FromSqlRaw("EXEC sp_ValidarUsuario @p0, @p1", request.Username, request.Password)
        .ToListAsync())
        .FirstOrDefault();

    if (user == null) return Unauthorized(new { message = "Credenciales inválidas" });

    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
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