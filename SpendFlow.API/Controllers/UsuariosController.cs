using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpendFlow.API.Data;
using SpendFlow.API.Models;

namespace SpendFlow.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsuariosController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<Usuario>> PostUsuario(Usuario usuario)
        {
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();
            return Ok(usuario);
        }
    }
}