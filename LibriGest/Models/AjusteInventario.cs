namespace LibriGest.Models
{
    public class AjusteInventario
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; } = DateTime.Now;
        public int AlmacenId { get; set; }
        public Almacen? Almacen { get; set; }
        public int UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }
        public string Tipo { get; set; } = "Entrada"; // Entrada, Salida
        public string Motivo { get; set; } = string.Empty;

        public ICollection<AjusteDetalle> Detalles { get; set; } = new List<AjusteDetalle>();
    }
}
