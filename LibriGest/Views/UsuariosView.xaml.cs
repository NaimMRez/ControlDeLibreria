using System.Windows;
using System.Windows.Controls;

namespace LibriGest.Views
{
    public partial class UsuariosView : UserControl
    {
        public UsuariosView()
        {
            InitializeComponent();
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
