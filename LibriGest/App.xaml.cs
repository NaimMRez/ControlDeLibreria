using LibriGest.Views;
using System.Windows;
using System.Windows.Threading;

namespace LibriGest
{
    public partial class App : Application
    {
        public App()
        {
            // Capturar excepciones no manejadas
            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                var ex = e.ExceptionObject as Exception;
                MessageBox.Show($"ERROR GLOBAL (AppDomain):\n\n{ex?.Message}\n\n{ex?.StackTrace}", 
                    "Error Fatal", MessageBoxButton.OK, MessageBoxImage.Error);
            };

            DispatcherUnhandledException += (sender, e) =>
            {
                MessageBox.Show($"ERROR EN UI THREAD:\n\n{e.Exception.Message}\n\n{e.Exception.StackTrace}", 
                    "Error Fatal", MessageBoxButton.OK, MessageBoxImage.Error);
                e.Handled = true;
            };
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            try
            {
                // Evitar que la app se cierre cuando el login (primer window) se cierra
                ShutdownMode = ShutdownMode.OnExplicitShutdown;

                // Inicializar base de datos
                using (var context = new Data.AppDbContext())
                {
                    context.Database.EnsureCreated();
                    Data.DbInitializer.Initialize(context);
                }

                // Mostrar login primero
                var loginWindow = new LoginWindow();
                bool? result = loginWindow.ShowDialog();
                
                if (result == true)
                {
                    var mainWindow = new MainWindow();
                    MainWindow = mainWindow;
                    mainWindow.Show();
                    ShutdownMode = ShutdownMode.OnMainWindowClose;
                }
                else
                {
                    Shutdown();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al iniciar la aplicación:\n\n{ex.Message}\n\nStackTrace:\n{ex.StackTrace}\n\nInner:{ex.InnerException?.Message}", 
                    "Error de Inicio", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown();
            }
        }
    }
}
