namespace SpendFlow.API.Models
{
    public class Usuario
    {
        public int Id { get; set; }

        // Tus campos originales
        public string Nombre { get; set; } = string.Empty;
        public decimal CapitalInicial { get; set; }

        // Campos para el sistema de Login y Registro
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;

        // Relación: Un usuario tiene muchos movimientos
        public List<Movimiento> Movimientos { get; set; } = new List<Movimiento>();
    }
}