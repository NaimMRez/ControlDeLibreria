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
            // Normalizar visibilidades por defecto
            ButtonGestionarCompras.Visibility = Visibility.Visible;
            ButtonRegistrarCompra.Visibility = Visibility.Visible;
            ButtonProveedores.Visibility = Visibility.Visible;
            ButtonProductos.Visibility = Visibility.Visible;
            ButtonAjustesInventario.Visibility = Visibility.Visible;
            ButtonCaja.Visibility = Visibility.Visible;
            ButtonGastos.Visibility = Visibility.Visible;
            ButtonConfiguracion.Visibility = Visibility.Visible;
            ButtonUsuarios.Visibility = Visibility.Visible;
            ButtonGestionarVentas.Visibility = Visibility.Visible;
            ButtonRegistrarVenta.Visibility = Visibility.Visible;
            ButtonClientes.Visibility = Visibility.Visible;

            var usuario = LibriGest.Helpers.SessionContext.CurrentUser;
            if (usuario == null)
            {
                MessageBox.Show("No se ha detectado un usuario en sesión. La aplicación se cerrará.", "Error de Sesión", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
                return;
            }

            TextBlockUsuario.Text = usuario.NombreCompleto;
            TextBlockRol.Text = usuario.Rol;

            // Control de acceso por rol
            if (usuario.Rol == "Vendedor")
            {
                ButtonGestionarCompras.Visibility = Visibility.Collapsed;
                ButtonRegistrarCompra.Visibility = Visibility.Collapsed;
                ButtonProveedores.Visibility = Visibility.Collapsed;
                ButtonProductos.Visibility = Visibility.Collapsed;
                ButtonAjustesInventario.Visibility = Visibility.Collapsed;
                ButtonCaja.Visibility = Visibility.Collapsed;
                ButtonGastos.Visibility = Visibility.Collapsed;
                ButtonConfiguracion.Visibility = Visibility.Collapsed;
                ButtonUsuarios.Visibility = Visibility.Collapsed;
            }
            else if (usuario.Rol == "Almacenero")
            {
                ButtonGestionarVentas.Visibility = Visibility.Collapsed;
                ButtonRegistrarVenta.Visibility = Visibility.Collapsed;
                ButtonClientes.Visibility = Visibility.Collapsed;
                ButtonGestionarCompras.Visibility = Visibility.Collapsed;
                ButtonProveedores.Visibility = Visibility.Collapsed;
                ButtonCaja.Visibility = Visibility.Collapsed;
                ButtonGastos.Visibility = Visibility.Collapsed;
                ButtonConfiguracion.Visibility = Visibility.Collapsed;
                ButtonUsuarios.Visibility = Visibility.Collapsed;
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
