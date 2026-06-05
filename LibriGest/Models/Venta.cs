namespace LibriGest.Models
{
    public class Venta
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; } = DateTime.Now;
        public int? ClienteId { get; set; }
        public Cliente? Cliente { get; set; }
        public int UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Descuento { get; set; }
        public decimal Total { get; set; }
        public string Estado { get; set; } = "Completada"; // Completada, Anulada
        public int? CajaId { get; set; }
        public Caja? Caja { get; set; }

        public ICollection<VentaDetalle> Detalles { get; set; } = new List<VentaDetalle>();
    }
}
