using System.Text.Json.Serialization;

namespace SpendFlow.API.Models
{
    public class Movimiento
    {
        public int Id { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public decimal Monto { get; set; }
        public DateTime Fecha { get; set; } = DateTime.UtcNow;
        public string Tipo { get; set; } = string.Empty; // Ej: "ingreso" o "gasto"
        public string Categoria { get; set; } = string.Empty;

        // Llave foránea para vincularlo al Usuario
        public int UsuarioId { get; set; } // Esto se queda igual

        [JsonIgnore]
        public Usuario? Usuario { get; set; }
    }
}