using LibriGest.Models;
using LibriGest.Views;
using System.Windows;

namespace LibriGest
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            CargarConfiguracion();
            CargarUsuarioActual();
            MainContentArea.Content = new DashboardView();
        }

        private void CargarConfiguracion()
        {
            using var context = new Data.AppDbContext();
            var nombreTienda = context.ConfiguracionSistema.FirstOrDefault(c => c.Clave == "NombreTienda")?.Valor ?? "Mi Librería";
            TextBlockNombreTienda.Text = nombreTienda;
            Title = $"LibriGest - {nombreTienda}";
        }

        private void CargarUsuarioActual()
        {
            if (Application.Current.Properties["UsuarioActual"] is Usuario usuario)
            {
                TextBlockUsuario.Text = usuario.NombreCompleto;
                TextBlockRol.Text = usuario.Rol;

                // Control de acceso por rol
                if (usuario.Rol == "Vendedor")
                {
                    // Ocultar opciones de administración
                    // Esto es básico, se puede mejorar con un sistema de permisos más granular
                }
            }
        }

        private void NavigateTo(object view)
        {
            MainContentArea.Content = view;
        }

        private void MenuDashboard_Click(object sender, RoutedEventArgs e) => NavigateTo(new DashboardView());
        private void MenuRegistrarVenta_Click(object sender, RoutedEventArgs e) => NavigateTo(new RegistrarVentaView());
        private void MenuGestionarVentas_Click(object sender, RoutedEventArgs e) => NavigateTo(new GestionarVentasView());
        private void MenuClientes_Click(object sender, RoutedEventArgs e) => NavigateTo(new ClientesView());
        private void MenuRegistrarCompra_Click(object sender, RoutedEventArgs e) => NavigateTo(new RegistrarCompraView());
        private void MenuGestionarCompras_Click(object sender, RoutedEventArgs e) => NavigateTo(new GestionarComprasView());
        private void MenuProveedores_Click(object sender, RoutedEventArgs e) => NavigateTo(new ProveedoresView());
        private void MenuProductos_Click(object sender, RoutedEventArgs e) => NavigateTo(new ProductosView());
        private void MenuAjustesInventario_Click(object sender, RoutedEventArgs e) => NavigateTo(new AjustesInventarioView());
        private void MenuCaja_Click(object sender, RoutedEventArgs e) => NavigateTo(new CajaView());
        private void MenuGastos_Click(object sender, RoutedEventArgs e) => NavigateTo(new GastosView());
        private void MenuConfiguracion_Click(object sender, RoutedEventArgs e) => NavigateTo(new ConfiguracionView());
        private void MenuUsuarios_Click(object sender, RoutedEventArgs e) => NavigateTo(new UsuariosView());
    }
}
