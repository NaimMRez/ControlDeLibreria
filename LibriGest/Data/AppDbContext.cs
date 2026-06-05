using Microsoft.EntityFrameworkCore;
using LibriGest.Models;

namespace LibriGest.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<ConfiguracionSistema> ConfiguracionSistema { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Unidad> Unidades { get; set; }
        public DbSet<Almacen> Almacenes { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Stock> Stocks { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Proveedor> Proveedores { get; set; }
        public DbSet<Venta> Ventas { get; set; }
        public DbSet<VentaDetalle> VentaDetalles { get; set; }
        public DbSet<Cobro> Cobros { get; set; }
        public DbSet<Compra> Compras { get; set; }
        public DbSet<CompraDetalle> CompraDetalles { get; set; }
        public DbSet<Pago> Pagos { get; set; }
        public DbSet<Caja> Cajas { get; set; }
        public DbSet<Gasto> Gastos { get; set; }
        public DbSet<AjusteInventario> AjustesInventario { get; set; }
        public DbSet<AjusteDetalle> AjusteDetalles { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            var dbPath = System.IO.Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "LibriGest",
                "librigest.db"
            );
            System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(dbPath)!);
            options.UseSqlite($"Data Source={dbPath}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ConfiguracionSistema>().HasIndex(c => c.Clave).IsUnique();
            modelBuilder.Entity<Producto>().HasIndex(p => p.CodigoBarras);
            modelBuilder.Entity<Cliente>().HasIndex(c => c.Documento);
            modelBuilder.Entity<Proveedor>().HasIndex(p => p.RUC);
        }
    }
}
