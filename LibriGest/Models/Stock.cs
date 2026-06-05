namespace LibriGest.Models
{
    public class Stock
    {
        public int Id { get; set; }
        public int ProductoId { get; set; }
        public Producto? Producto { get; set; }
        public int AlmacenId { get; set; }
        public Almacen? Almacen { get; set; }
        public int Cantidad { get; set; }
    }
}
