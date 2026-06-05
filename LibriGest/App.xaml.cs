using LibriGest.Views;
using System.Windows;
using System.Windows.Threading;

namespace LibriGest
{
    public partial class App : Application
    {
        public App()
        {
            // DIAGNÓSTICO 1: Verificar que el constructor de App se ejecuta
            MessageBox.Show("Diagnóstico 1: Constructor de App ejecutado. Presione OK para continuar.", 
                "LibriGest - Diagnóstico", MessageBoxButton.OK, MessageBoxImage.Information);

            // Capturar TODAS las excepciones no manejadas
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

            // DIAGNÓSTICO 2: Verificar que OnStartup se ejecuta
            MessageBox.Show("Diagnóstico 2: OnStartup ejecutado. Presione OK para inicializar la base de datos.", 
                "LibriGest - Diagnóstico", MessageBoxButton.OK, MessageBoxImage.Information);

            try
            {
                // Inicializar base de datos
                using (var context = new Data.AppDbContext())
                {
                    context.Database.EnsureCreated();
                    Data.DbInitializer.Initialize(context);
                }

                // DIAGNÓSTICO 3: Base de datos OK
                MessageBox.Show("Diagnóstico 3: Base de datos inicializada correctamente. Presione OK para abrir el login.", 
                    "LibriGest - Diagnóstico", MessageBoxButton.OK, MessageBoxImage.Information);

                // Mostrar login primero
                var loginWindow = new LoginWindow();
                bool? result = loginWindow.ShowDialog();
                
                if (result == true)
                {
                    var mainWindow = new MainWindow();
                    mainWindow.Show();
                }
                else
                {
                    MessageBox.Show("Diagnóstico 4: Login cancelado o cerrado. La aplicación se cerrará.", 
                        "LibriGest - Diagnóstico", MessageBoxButton.OK, MessageBoxImage.Information);
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
