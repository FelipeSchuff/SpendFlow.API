using Microsoft.EntityFrameworkCore;
using SpendFlow.API.Models;

namespace SpendFlow.API.Data
{
    public class AppDbContext : DbContext
    {
        // El constructor recibe las opciones de configuración (como la cadena de conexión)
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Estas propiedades representan las tablas en tu base de datos
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Movimiento> Movimientos { get; set; }
    }
}