using System.Windows.Controls;

namespace LibriGest.Views
{
    public partial class ConfiguracionView : UserControl
    {
        public ConfiguracionView()
        {
            InitializeComponent();

            if (!Helpers.Authorization.HasPermission("Configuracion"))
            {
                IsEnabled = false;
                System.Windows.MessageBox.Show("Acceso denegado: necesita permisos de administrador para ver configuración.", "Acceso", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                return;
            }

            DataContext = new ViewModels.ConfiguracionViewModel();
        }
    }
}
