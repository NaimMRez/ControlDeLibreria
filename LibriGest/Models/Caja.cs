namespace LibriGest.Models
{
    public class Caja
    {
        public int Id { get; set; }
        public DateTime FechaApertura { get; set; } = DateTime.Now;
        public DateTime? FechaCierre { get; set; }
        public int UsuarioAperturaId { get; set; }
        public Usuario? UsuarioApertura { get; set; }
        public int? UsuarioCierreId { get; set; }
        public Usuario? UsuarioCierre { get; set; }
        public decimal MontoInicial { get; set; }
        public decimal TotalVentas { get; set; }
        public decimal TotalGastos { get; set; }
        public decimal? MontoCierre { get; set; }
        public string Estado { get; set; } = "Abierta"; // Abierta, Cerrada
    }
}
