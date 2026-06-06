using System.Windows.Controls;
using System.Windows.Input;

namespace LibriGest.Views
{
    public partial class RegistrarCompraView : UserControl
    {
        public RegistrarCompraView()
        {
            InitializeComponent();

            if (!Helpers.Authorization.HasPermission("RegistrarCompra"))
            {
                IsEnabled = false;
                System.Windows.MessageBox.Show("Acceso denegado: no tiene permiso para registrar compras.", "Acceso", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                return;
            }

            DataContext = new ViewModels.RegistrarCompraViewModel();
        }

        private void TextBoxCodigoBarras_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return || e.Key == Key.Enter)
            {
                if (DataContext is ViewModels.RegistrarCompraViewModel vm)
                {
                    vm.BuscarProductoPorCodigo();
                    TextBoxCodigoBarras.Focus();
                    TextBoxCodigoBarras.SelectAll();
                }
                e.Handled = true;
            }
        }
    }
}
