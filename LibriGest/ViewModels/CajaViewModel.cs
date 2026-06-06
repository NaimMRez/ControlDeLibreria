using LibriGest.Models;
using LibriGest.Helpers;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace LibriGest.ViewModels
{
    public class CajaViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private Caja? _cajaActual;
        private decimal _montoApertura = 0;
        private decimal _montoCierre = 0;
        private bool _mostrarApertura = true;
        private bool _mostrarCierre = false;
        private ObservableCollection<Caja> _cajasHistoricas = new();

        public Caja? CajaActual
        {
            get => _cajaActual;
            set { _cajaActual = value; OnPropertyChanged(nameof(CajaActual)); }
        }

        public decimal MontoApertura
        {
            get => _montoApertura;
            set { _montoApertura = value; OnPropertyChanged(nameof(MontoApertura)); }
        }

        public decimal MontoCierre
        {
            get => _montoCierre;
            set { _montoCierre = value; OnPropertyChanged(nameof(MontoCierre)); }
        }

        public bool MostrarApertura
        {
            get => _mostrarApertura;
            set { _mostrarApertura = value; OnPropertyChanged(nameof(MostrarApertura)); }
        }

        public bool MostrarCierre
        {
            get => _mostrarCierre;
            set { _mostrarCierre = value; OnPropertyChanged(nameof(MostrarCierre)); }
        }

        public ObservableCollection<Caja> CajasHistoricas
        {
            get => _cajasHistoricas;
            set { _cajasHistoricas = value; OnPropertyChanged(nameof(CajasHistoricas)); }
        }

        public ICommand ComandoIniciarCaja { get; }
        public ICommand ComandoCerrarCaja { get; }
        public ICommand ComandoRefrescar { get; }

        public CajaViewModel()
        {
            ComandoIniciarCaja = new RelayCommand(_ => IniciarCaja());
            ComandoCerrarCaja = new RelayCommand(_ => CerrarCaja());
            ComandoRefrescar = new RelayCommand(_ => CargarDatos());

            CargarDatos();
        }

        private void CargarDatos()
        {
            using var context = new Data.AppDbContext();
            CajaActual = context.Cajas.FirstOrDefault(c => c.Estado == "Abierta");

            if (CajaActual != null)
            {
                MostrarApertura = false;
                MostrarCierre = true;

                // Calcular totales actuales
                var ventas = context.Ventas.Where(v => v.CajaId == CajaActual.Id && v.Estado == "Completada").ToList().Sum(v => (decimal?)v.Total) ?? 0;
                var gastos = context.Gastos.Where(g => g.CajaId == CajaActual.Id).ToList().Sum(g => (decimal?)g.Monto) ?? 0;

                CajaActual.TotalVentas = ventas;
                CajaActual.TotalGastos = gastos;
                MontoCierre = CajaActual.MontoInicial + ventas - gastos;
            }
            else
            {
                MostrarApertura = true;
                MostrarCierre = false;
                MontoApertura = 0;
            }

            // Cargar histórico
            var historico = context.Cajas.Where(c => c.Estado == "Cerrada").OrderByDescending(c => c.FechaApertura).Take(20).ToList();
            CajasHistoricas.Clear();
            foreach (var c in historico) CajasHistoricas.Add(c);
        }

        private void IniciarCaja()
        {
            if (MontoApertura < 0)
            {
                MessageBox.Show("El monto de apertura no puede ser negativo.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using var context = new Data.AppDbContext();
            if (context.Cajas.Any(c => c.Estado == "Abierta"))
            {
                MessageBox.Show("Ya existe una caja abierta.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                CargarDatos();
                return;
            }

            var nuevaCaja = new Caja
            {
                FechaApertura = DateTime.Now,
                UsuarioAperturaId = SessionContext.CurrentUserId,
                MontoInicial = MontoApertura,
                Estado = "Abierta"
            };

            context.Cajas.Add(nuevaCaja);
            context.SaveChanges();

            MessageBox.Show("Caja iniciada correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
            CargarDatos();
        }

        private void CerrarCaja()
        {
            if (CajaActual == null) return;

            var resultado = MessageBox.Show(
                $"¿Cerrar caja?\n\nMonto inicial: {CajaActual.MontoInicial:C}\nVentas: {CajaActual.TotalVentas:C}\nGastos: {CajaActual.TotalGastos:C}\n\nEfectivo esperado: {MontoCierre:C}",
                "Confirmar cierre", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (resultado == MessageBoxResult.Yes)
            {
                using var context = new Data.AppDbContext();
                var caja = context.Cajas.Find(CajaActual.Id);
                if (caja != null)
                {
                    caja.FechaCierre = DateTime.Now;
                    caja.UsuarioCierreId = SessionContext.CurrentUserId;
                    caja.TotalVentas = CajaActual.TotalVentas;
                    caja.TotalGastos = CajaActual.TotalGastos;
                    caja.MontoCierre = MontoCierre;
                    caja.Estado = "Cerrada";
                    context.SaveChanges();
                }

                MessageBox.Show("Caja cerrada correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                CargarDatos();
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
