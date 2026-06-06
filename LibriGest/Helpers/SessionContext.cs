using LibriGest.Models;
using System.Windows;

namespace LibriGest.Helpers
{
    public static class SessionContext
    {
        public static Usuario? CurrentUser
        {
            get
            {
                if (Application.Current?.Properties["UsuarioActual"] is Usuario usuario)
                {
                    return usuario;
                }

                return null;
            }
        }

        public static int CurrentUserId => CurrentUser?.Id ?? 0;

        public static void SetCurrentUser(Usuario? usuario)
        {
            if (Application.Current != null)
            {
                Application.Current.Properties["UsuarioActual"] = usuario;
            }
        }
    }
}