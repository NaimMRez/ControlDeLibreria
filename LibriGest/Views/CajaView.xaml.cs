using System.Windows;
using System.Windows.Controls;

namespace LibriGest.Views
{
    public partial class CajaView : UserControl
    {
        public CajaView()
        {
            InitializeComponent();

            if (!Helpers.Authorization.HasPermission("Caja"))
            {
                IsEnabled = false;
                MessageBox.Show("Acceso denegado: no tiene permiso para gestionar la caja.", "Acceso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DataContext = new ViewModels.CajaViewModel();
        }
    }
}
