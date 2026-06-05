using LibriGest.Models;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace LibriGest.ViewModels
{
    public class ConfiguracionViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private string _nombreTienda = "";
        private string _direccion = "";
        private string _telefono = "";
        private string _moneda = "S/";

        public string NombreTienda
        {
            get => _nombreTienda;
            set { _nombreTienda = value; OnPropertyChanged(nameof(NombreTienda)); }
        }

        public string Direccion
        {
            get => _direccion;
            set { _direccion = value; OnPropertyChanged(nameof(Direccion)); }
        }

        public string Telefono
        {
            get => _telefono;
            set { _telefono = value; OnPropertyChanged(nameof(Telefono)); }
        }

        public string Moneda
        {
            get => _moneda;
            set { _moneda = value; OnPropertyChanged(nameof(Moneda)); }
        }

        public ICommand ComandoGuardar { get; }

        public ConfiguracionViewModel()
        {
            ComandoGuardar = new RelayCommand(_ => Guardar());
            CargarConfiguracion();
        }

        private void CargarConfiguracion()
        {
            using var context = new Data.AppDbContext();
            NombreTienda = GetValor(context, "NombreTienda");
            Direccion = GetValor(context, "Direccion");
            Telefono = GetValor(context, "Telefono");
            Moneda = GetValor(context, "Moneda");
        }

        private string GetValor(Data.AppDbContext context, string clave)
        {
            return context.ConfiguracionSistema.FirstOrDefault(c => c.Clave == clave)?.Valor ?? "";
        }

        private void Guardar()
        {
            using var context = new Data.AppDbContext();
            SetValor(context, "NombreTienda", NombreTienda);
            SetValor(context, "Direccion", Direccion);
            SetValor(context, "Telefono", Telefono);
            SetValor(context, "Moneda", Moneda);
            context.SaveChanges();

            MessageBox.Show("Configuración guardada correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void SetValor(Data.AppDbContext context, string clave, string valor)
        {
            var config = context.ConfiguracionSistema.FirstOrDefault(c => c.Clave == clave);
            if (config != null)
            {
                config.Valor = valor;
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
