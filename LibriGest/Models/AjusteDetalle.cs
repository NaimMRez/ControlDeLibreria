namespace LibriGest.Models
{
    public class AjusteDetalle
    {
        public int Id { get; set; }
        public int AjusteInventarioId { get; set; }
        public AjusteInventario? AjusteInventario { get; set; }
        public int ProductoId { get; set; }
        public Producto? Producto { get; set; }
        public int CantidadAnterior { get; set; }
        public int CantidadNueva { get; set; }
        public int Diferencia { get; set; }
    }
}
