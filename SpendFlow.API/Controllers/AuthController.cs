using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpendFlow.API.Data;
using SpendFlow.API.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SpendFlow.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        // Inyectamos la base de datos y la configuración (para leer la clave secreta del JWT)
        public AuthController(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [HttpPost("registro")]
        public async Task<IActionResult> Registro([FromBody] RegisterRequest request)
        {
            // Verificamos si el correo ya existe
            if (await _context.Usuarios.AnyAsync(u => u.Email == request.Email))
            {
                return BadRequest("El correo ya está registrado.");
            }

            // Creamos el nuevo usuario encriptando su contraseña
            var nuevoUsuario = new Usuario
            {
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Nombre = "Usuario", // Valor por defecto
                CapitalInicial = 0  // Valor por defecto, se cambia después en la app
            };

            _context.Usuarios.Add(nuevoUsuario);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Usuario registrado con éxito." });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            // Buscamos al usuario por correo
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == request.Email);

            if (usuario == null)
            {
                return Unauthorized("Credenciales incorrectas.");
            }

            // Comparamos la contraseña enviada con el Hash guardado en la BD
            if (!BCrypt.Net.BCrypt.Verify(request.Password, usuario.PasswordHash))
            {
                return Unauthorized("Credenciales incorrectas.");
            }

            // Generamos el token si todo está bien
            var token = GenerarTokenJWT(usuario);

            return Ok(new { token = token });
        }

        // Método auxiliar para generar el Token JWT
        private string GenerarTokenJWT(Usuario usuario)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // Aquí guardamos el ID del usuario dentro del token para usarlo luego en los gastos
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, usuario.Email)
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(2), // El token dura 2 horas
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}