using LibriGest.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace LibriGest.ViewModels
{
    public class GestionarVentasViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private string _busqueda = "";
        private DateTime _fechaDesde = DateTime.Now.AddDays(-7);
        private DateTime _fechaHasta = DateTime.Now;
        private Venta? _ventaSeleccionada;
        private bool _mostrarDetalle;

        public string Busqueda
        {
            get => _busqueda;
            set { _busqueda = value; OnPropertyChanged(nameof(Busqueda)); FiltrarVentas(); }
        }

        public DateTime FechaDesde
        {
            get => _fechaDesde;
            set { _fechaDesde = value; OnPropertyChanged(nameof(FechaDesde)); FiltrarVentas(); }
        }

        public DateTime FechaHasta
        {
            get => _fechaHasta;
            set { _fechaHasta = value; OnPropertyChanged(nameof(FechaHasta)); FiltrarVentas(); }
        }

        public ObservableCollection<Venta> Ventas { get; set; } = new();
        public ObservableCollection<VentaDetalle> VentaDetalles { get; set; } = new();

        public Venta? VentaSeleccionada
        {
            get => _ventaSeleccionada;
            set
            {
                _ventaSeleccionada = value;
                OnPropertyChanged(nameof(VentaSeleccionada));
                CargarDetalleVenta();
            }
        }

        public bool MostrarDetalle
        {
            get => _mostrarDetalle;
            set { _mostrarDetalle = value; OnPropertyChanged(nameof(MostrarDetalle)); OnPropertyChanged(nameof(MostrarLista)); }
        }

        public bool MostrarLista => !_mostrarDetalle;

        public ICommand ComandoVerDetalle { get; }
        public ICommand ComandoAnular { get; }
        public ICommand ComandoCerrarDetalle { get; }

        public GestionarVentasViewModel()
        {
            ComandoVerDetalle = new RelayCommand(_ => MostrarDetalle = true, _ => VentaSeleccionada != null);
            ComandoAnular = new RelayCommand(_ => AnularVenta(), _ => VentaSeleccionada != null && VentaSeleccionada.Estado == "Completada");
            ComandoCerrarDetalle = new RelayCommand(_ => MostrarDetalle = false);

            FiltrarVentas();
        }

        private void FiltrarVentas()
        {
            using var context = new Data.AppDbContext();
            var query = context.Ventas
                .Where(v => v.Fecha >= FechaDesde && v.Fecha <= FechaHasta.AddDays(1))
                .OrderByDescending(v => v.Fecha);

            if (!string.IsNullOrWhiteSpace(Busqueda))
            {
                var busquedaLower = Busqueda.ToLower();
                query = (IOrderedQueryable<Venta>)query.Where(v =>
                    (v.Cliente != null && v.Cliente.Nombre.ToLower().Contains(busquedaLower)) ||
                    v.Id.ToString().Contains(Busqueda)
                );
            }

            var lista = query.ToList();
            Ventas.Clear();
            foreach (var v in lista) Ventas.Add(v);
        }

        private void CargarDetalleVenta()
        {
            VentaDetalles.Clear();
            if (VentaSeleccionada == null) return;

            using var context = new Data.AppDbContext();
            var detalles = context.VentaDetalles
                .Where(d => d.VentaId == VentaSeleccionada.Id)
                .ToList();

            foreach (var d in detalles) VentaDetalles.Add(d);
        }

        private void AnularVenta()
        {
            if (VentaSeleccionada == null) return;

            if (MessageBox.Show($"¿Anular la venta #{VentaSeleccionada.Id} por {VentaSeleccionada.Total:C}?", "Confirmar", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                using var context = new Data.AppDbContext();
                using var transaction = context.Database.BeginTransaction();

                try
                {
                    var venta = context.Ventas.Find(VentaSeleccionada.Id);
                    if (venta != null)
                    {
                        venta.Estado = "Anulada";

                        // Revertir stock
                        var detalles = context.VentaDetalles.Where(d => d.VentaId == venta.Id).ToList();
                        foreach (var detalle in detalles)
                        {
                            var stock = context.Stocks.FirstOrDefault(s => s.ProductoId == detalle.ProductoId);
                            if (stock != null)
                            {
                                stock.Cantidad += detalle.Cantidad;
                            }
                        }

                        context.SaveChanges();
                        transaction.Commit();

                        MessageBox.Show("Venta anulada correctamente. El stock ha sido restaurado.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                        FiltrarVentas();
                        MostrarDetalle = false;
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    MessageBox.Show($"Error al anular: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
