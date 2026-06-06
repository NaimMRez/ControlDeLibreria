using LibriGest.Models;
using System.Windows;
using BC = BCrypt.Net.BCrypt;

namespace LibriGest
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void ButtonIngresar_Click(object sender, RoutedEventArgs e)
        {
            var usuario = TextBoxUsuario.Text.Trim();
            var clave = PasswordBoxClave.Password;

            if (string.IsNullOrWhiteSpace(usuario) || string.IsNullOrWhiteSpace(clave))
            {
                TextBlockError.Text = "Ingrese usuario y contraseña.";
                return;
            }

            using var context = new Data.AppDbContext();
            var user = context.Usuarios.FirstOrDefault(u => u.NombreUsuario == usuario && u.Activo);

            if (user != null && BC.Verify(clave, user.ClaveHash))
            {
                // Guardar usuario actual en sesión (simplificado)
                LibriGest.Helpers.SessionContext.SetCurrentUser(user);
                DialogResult = true;
                Close();
            }
            else
            {
                TextBlockError.Text = "Usuario o contraseña incorrectos.";
            }
        }
    }
}
