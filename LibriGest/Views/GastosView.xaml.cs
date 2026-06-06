using System.Windows;
using System.Windows.Controls;

namespace LibriGest.Views
{
    public partial class GastosView : UserControl
    {
        public GastosView()
        {
            InitializeComponent();

            if (!Helpers.Authorization.HasPermission("Gastos"))
            {
                IsEnabled = false;
                MessageBox.Show("Acceso denegado: no tiene permiso para gestionar gastos.", "Acceso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DataContext = new ViewModels.GastosViewModel();
        }
    }
}
