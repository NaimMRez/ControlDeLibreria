using System.Windows;
using System.Windows.Controls;

namespace LibriGest.Views
{
    public partial class ProductosView : UserControl
    {
        public ProductosView()
        {
            InitializeComponent();

            if (!Helpers.Authorization.HasPermission("Productos"))
            {
                IsEnabled = false;
                MessageBox.Show("Acceso denegado: no tiene permiso para ver productos.", "Acceso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DataContext = new ViewModels.ProductosViewModel();
        }
    }
}
