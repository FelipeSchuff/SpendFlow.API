namespace SpendFlow.API.Models
{
    public class MovimientoDto
    {
        public string Descripcion { get; set; } = string.Empty;
        public decimal Monto { get; set; }
        public DateTime Fecha { get; set; } = DateTime.UtcNow;
        public string Tipo { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
    }
}