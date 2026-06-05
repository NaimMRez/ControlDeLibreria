namespace LibriGest.Models
{
    public class Pago
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; } = DateTime.Now;
        public int ProveedorId { get; set; }
        public Proveedor? Proveedor { get; set; }
        public int? CompraId { get; set; }
        public Compra? Compra { get; set; }
        public decimal Monto { get; set; }
        public string MetodoPago { get; set; } = "Efectivo";
        public string? Notas { get; set; }
    }
}
