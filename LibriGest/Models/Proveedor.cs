namespace LibriGest.Models
{
    public class Proveedor
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? RUC { get; set; }
        public string? Telefono { get; set; }
        public string? Direccion { get; set; }
        public string? Email { get; set; }
        public DateTime FechaRegistro { get; set; } = DateTime.Now;
    }
}
