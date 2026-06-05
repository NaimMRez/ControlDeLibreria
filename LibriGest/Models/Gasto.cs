namespace LibriGest.Models
{
    public class Gasto
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; } = DateTime.Now;
        public int CajaId { get; set; }
        public Caja? Caja { get; set; }
        public string Concepto { get; set; } = string.Empty;
        public decimal Monto { get; set; }
        public string Categoria { get; set; } = "General";
        public string? Comprobante { get; set; }
        public int UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }
    }
}
