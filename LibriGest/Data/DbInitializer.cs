using LibriGest.Models;
using BC = BCrypt.Net.BCrypt;

namespace LibriGest.Data
{
    public static class DbInitializer
    {
        public static void Initialize(AppDbContext context)
        {
            context.Database.EnsureCreated();

            // Usuario admin por defecto
            if (!context.Usuarios.Any(u => u.NombreUsuario == "admin"))
            {
                context.Usuarios.Add(new Usuario
                {
                    NombreUsuario = "admin",
                    NombreCompleto = "Administrador",
                    ClaveHash = BC.HashPassword("admin123"),
                    Rol = "Administrador",
                    Activo = true
                });
            }

            // Usuario vendedor por defecto
            if (!context.Usuarios.Any(u => u.NombreUsuario == "vendedor"))
            {
                context.Usuarios.Add(new Usuario
                {
                    NombreUsuario = "vendedor",
                    NombreCompleto = "Vendedor Default",
                    ClaveHash = BC.HashPassword("vendedor123"),
                    Rol = "Vendedor",
                    Activo = true
                });
            }

            // Configuración default
            if (!context.ConfiguracionSistema.Any())
            {
                context.ConfiguracionSistema.AddRange(
                    new ConfiguracionSistema { Clave = "NombreTienda", Valor = "Mi Librería" },
                    new ConfiguracionSistema { Clave = "Direccion", Valor = "" },
                    new ConfiguracionSistema { Clave = "Telefono", Valor = "" },
                    new ConfiguracionSistema { Clave = "Moneda", Valor = "S/" },
                    new ConfiguracionSistema { Clave = "Impuesto", Valor = "0" }
                );
            }

            // Almacén default
            if (!context.Almacenes.Any())
            {
                context.Almacenes.Add(new Almacen { Nombre = "Tienda Principal", Ubicacion = "Local comercial" });
            }

            // Categorías default
            if (!context.Categorias.Any())
            {
                context.Categorias.AddRange(
                    new Categoria { Nombre = "Libros", Descripcion = "Libros de todas las áreas" },
                    new Categoria { Nombre = "Cuadernos", Descripcion = "Cuadernos y block de notas" },
                    new Categoria { Nombre = "Útiles Escolares", Descripcion = "Lápices, borradores, etc." },
                    new Categoria { Nombre = "Papelería", Descripcion = "Papel, carpetas, etc." }
                );
            }

            // Unidades default
            if (!context.Unidades.Any())
            {
                context.Unidades.AddRange(
                    new Unidad { Nombre = "Unidad", Simbolo = "Und" },
                    new Unidad { Nombre = "Docena", Simbolo = "Doc" },
                    new Unidad { Nombre = "Caja", Simbolo = "Cja" }
                );
            }

            context.SaveChanges();
        }
    }
}
