using LibriGest.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace LibriGest.ViewModels
{
    public class GastosViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private ObservableCollection<Gasto> _gastos = new();
        private Caja? _cajaAbierta;
        private string _concepto = "";
        private decimal _monto = 0;
        private string _categoria = "General";
        private string? _comprobante;

        public ObservableCollection<Gasto> Gastos
        {
            get => _gastos;
            set { _gastos = value; OnPropertyChanged(nameof(Gastos)); }
        }

        public Caja? CajaAbierta
        {
            get => _cajaAbierta;
            set { _cajaAbierta = value; OnPropertyChanged(nameof(CajaAbierta)); }
        }

        public string Concepto
        {
            get => _concepto;
            set { _concepto = value; OnPropertyChanged(nameof(Concepto)); }
        }

        public decimal Monto
        {
            get => _monto;
            set { _monto = value; OnPropertyChanged(nameof(Monto)); }
        }

        public string Categoria
        {
            get => _categoria;
            set { _categoria = value; OnPropertyChanged(nameof(Categoria)); }
        }

        public string? Comprobante
        {
            get => _comprobante;
            set { _comprobante = value; OnPropertyChanged(nameof(Comprobante)); }
        }

        public ICommand ComandoRegistrar { get; }
        public ICommand ComandoEliminar { get; }

        public GastosViewModel()
        {
            ComandoRegistrar = new RelayCommand(_ => RegistrarGasto(), _ => CajaAbierta != null);
            ComandoEliminar = new RelayCommand(param => EliminarGasto((Gasto)param!));

            CargarDatos();
        }

        private void CargarDatos()
        {
            using var context = new Data.AppDbContext();
            CajaAbierta = context.Cajas.FirstOrDefault(c => c.Estado == "Abierta");

            var lista = context.Gastos
                .Where(g => g.CajaId == (CajaAbierta?.Id ?? 0))
                .OrderByDescending(g => g.Fecha)
                .ToList();

            Gastos.Clear();
            foreach (var g in lista) Gastos.Add(g);
        }

        private void RegistrarGasto()
        {
            if (CajaAbierta == null)
            {
                MessageBox.Show("No hay una caja abierta. Abre la caja primero.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(Concepto) || Monto <= 0)
            {
                MessageBox.Show("Ingrese un concepto y monto válido.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using var context = new Data.AppDbContext();
            var gasto = new Gasto
            {
                Fecha = DateTime.Now,
                CajaId = CajaAbierta.Id,
                Concepto = Concepto,
                Monto = Monto,
                Categoria = Categoria,
                Comprobante = Comprobante,
                UsuarioId = 1 // TODO: Usuario actual
            };

            context.Gastos.Add(gasto);
            context.SaveChanges();

            Concepto = "";
            Monto = 0;
            Comprobante = null;
            OnPropertyChanged(string.Empty);

            CargarDatos();
            MessageBox.Show("Gasto registrado correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void EliminarGasto(Gasto gasto)
        {
            if (MessageBox.Show("¿Eliminar este gasto?", "Confirmar", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                using var context = new Data.AppDbContext();
                var g = context.Gastos.Find(gasto.Id);
                if (g != null)
                {
                    context.Gastos.Remove(g);
                    context.SaveChanges();
                }
                CargarDatos();
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
