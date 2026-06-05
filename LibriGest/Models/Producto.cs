namespace LibriGest.Models
{
    public class Producto
    {
        public int Id { get; set; }
        public string? CodigoBarras { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public int CategoriaId { get; set; }
        public Categoria? Categoria { get; set; }
        public int UnidadId { get; set; }
        public Unidad? Unidad { get; set; }
        public decimal PrecioCompra { get; set; }
        public decimal PrecioVenta { get; set; }
        public int StockMinimo { get; set; }
        public bool Activo { get; set; } = true;

        public ICollection<Stock> Stocks { get; set; } = new List<Stock>();
    }
}
