using System.Windows;
using System.Windows.Controls;

namespace LibriGest.Views
{
    public partial class UsuariosView : UserControl
    {
        public UsuariosView()
        {
            InitializeComponent();

            if (!Helpers.Authorization.HasPermission("Usuarios"))
            {
                IsEnabled = false;
                System.Windows.MessageBox.Show("Acceso denegado: necesita permisos para gestionar usuarios.", "Acceso", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                return;
            }

            DataContext = new ViewModels.UsuariosViewModel();
        }

        private void ButtonGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is ViewModels.UsuariosViewModel vm)
            {
                // Pasar la contraseña desde el PasswordBox al ViewModel
                vm.FormClave = PasswordBoxClave.Password;
                vm.ComandoGuardar.Execute(null);
                PasswordBoxClave.Clear();
            }
        }
    }
}
