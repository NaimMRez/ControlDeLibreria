using System.Windows.Controls;
using System.Windows.Input;

namespace LibriGest.Views
{
    public partial class RegistrarVentaView : UserControl
    {
        public RegistrarVentaView()
        {
            InitializeComponent();

            if (!Helpers.Authorization.HasPermission("RegistrarVenta"))
            {
                IsEnabled = false;
                System.Windows.MessageBox.Show("Acceso denegado: no tiene permiso para registrar ventas.", "Acceso", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                return;
            }

            DataContext = new ViewModels.RegistrarVentaViewModel();
        }

        private void TextBoxCodigoBarras_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // El lector de código de barras suele enviar Enter (Return) al final
            if (e.Key == Key.Return || e.Key == Key.Enter)
            {
                if (DataContext is ViewModels.RegistrarVentaViewModel vm)
                {
                    vm.BuscarProductoPorCodigo();
                    // Mantener el foco en el textbox para siguiente escaneo
                    TextBoxCodigoBarras.Focus();
                    TextBoxCodigoBarras.SelectAll();
                }
                e.Handled = true;
            }
        }
    }
}
