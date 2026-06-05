namespace LibriGest.Models
{
    public class Compra
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; } = DateTime.Now;
        public int ProveedorId { get; set; }
        public Proveedor? Proveedor { get; set; }
        public int UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Total { get; set; }
        public string Estado { get; set; } = "Completada"; // Completada, Anulada
        public int AlmacenId { get; set; }
        public Almacen? Almacen { get; set; }

        public ICollection<CompraDetalle> Detalles { get; set; } = new List<CompraDetalle>();
    }
}
