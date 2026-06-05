namespace LibriGest.Models
{
    public class Cobro
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; } = DateTime.Now;
        public int ClienteId { get; set; }
        public Cliente? Cliente { get; set; }
        public int? VentaId { get; set; }
        public Venta? Venta { get; set; }
        public decimal Monto { get; set; }
        public string MetodoPago { get; set; } = "Efectivo";
        public string? Notas { get; set; }
    }
}
