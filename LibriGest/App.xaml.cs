using LibriGest.Views;
using System.Windows;
using BC = BCrypt.Net.BCrypt;

namespace LibriGest
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Inicializar base de datos
            using (var context = new Data.AppDbContext())
            {
                context.Database.EnsureCreated();
                Data.DbInitializer.Initialize(context);
            }

            // Mostrar login primero
            var loginWindow = new LoginWindow();
            if (loginWindow.ShowDialog() == true)
            {
                var mainWindow = new MainWindow();
                mainWindow.Show();
            }
            else
            {
                Shutdown();
            }
        }
    }
}
