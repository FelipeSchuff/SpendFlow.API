using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpendFlow.API.Data;
using SpendFlow.API.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims; // Importamos esta librería para leer los datos del Token

namespace SpendFlow.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // EL CANDADO está activo
    public class MovimientosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MovimientosController(AppDbContext context)
        {
            _context = context;
        }

        // 1. GET: api/Movimientos -> (Ahora con filtros opcionales de Mes y Año)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Movimiento>>> GetMovimientos([FromQuery] int? mes, [FromQuery] int? anio)
        {
            // Extrae el ID del usuario directamente del Token validado
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // 1. Creamos la consulta base SOLO para este usuario (aún no va a la base de datos, es AsQueryable)
            var query = _context.Movimientos.Where(m => m.UsuarioId.ToString() == userId).AsQueryable();

            // 2. Si React nos envía un año, le agregamos ese filtro a la consulta
            if (anio.HasValue)
            {
                query = query.Where(m => m.Fecha.Year == anio.Value);
            }

            // 3. Si React nos envía un mes, le agregamos ese filtro a la consulta
            if (mes.HasValue)
            {
                query = query.Where(m => m.Fecha.Month == mes.Value);
            }

            // 4. Ahora sí, ejecutamos la consulta en PostgreSQL y ordenamos de más nuevo a más antiguo
            var misMovimientos = await query.OrderByDescending(m => m.Fecha).ToListAsync();

            return Ok(misMovimientos);
        }

        // 2. POST: api/Movimientos -> (Asigna el dueño automáticamente antes de guardar)
        [HttpPost]
        public async Task<ActionResult<Movimiento>> PostMovimiento(Movimiento movimiento)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // ANTES de guardarlo en la base de datos, le estampamos el ID del usuario
            // (Asumiendo que UsuarioId es de tipo int en tu modelo)
            movimiento.UsuarioId = int.Parse(userId!);

            _context.Movimientos.Add(movimiento);
            await _context.SaveChangesAsync();

            return Ok(movimiento);
        }

        // 3. PUT: api/Movimientos/5 -> (Protegido para que nadie edite gastos ajenos)
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMovimiento(int id, MovimientoDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Buscamos el movimiento REAL en la base de datos, no confiamos en lo que manda el cliente
            var movimientoExistente = await _context.Movimientos.FindAsync(id);

            if (movimientoExistente == null)
            {
                return NotFound("El movimiento que intentas actualizar no existe.");
            }

            // Comparamos contra el UsuarioId que YA ESTABA en la base de datos
            if (movimientoExistente.UsuarioId.ToString() != userId)
            {
                return Forbid("No tienes permiso para modificar este registro.");
            }

            // Solo actualizamos los campos permitidos, uno por uno
            movimientoExistente.Descripcion = dto.Descripcion;
            movimientoExistente.Monto = dto.Monto;
            movimientoExistente.Fecha = dto.Fecha;
            movimientoExistente.Tipo = dto.Tipo;
            movimientoExistente.Categoria = dto.Categoria;

            await _context.SaveChangesAsync();

            return Ok(movimientoExistente);
        }
        // 4. DELETE: api/Movimientos/5 -> (Protegido para que nadie borre gastos ajenos)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMovimiento(int id)
        {
            var movimiento = await _context.Movimientos.FindAsync(id);
            if (movimiento == null)
            {
                return NotFound("El movimiento que intentas borrar no existe.");
            }

            // Capa de seguridad extra: verificar que el registro le pertenece antes de borrarlo
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (movimiento.UsuarioId.ToString() != userId)
            {
                return Forbid(); // Status 403: Prohibido
            }

            _context.Movimientos.Remove(movimiento);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}