using LibriGest.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace LibriGest.ViewModels
{
    public class GestionarComprasViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private string _busqueda = "";
        private DateTime _fechaDesde = DateTime.Now.AddDays(-7);
        private DateTime _fechaHasta = DateTime.Now;
        private Compra? _compraSeleccionada;
        private bool _mostrarDetalle;

        public string Busqueda
        {
            get => _busqueda;
            set { _busqueda = value; OnPropertyChanged(nameof(Busqueda)); FiltrarCompras(); }
        }

        public DateTime FechaDesde
        {
            get => _fechaDesde;
            set { _fechaDesde = value; OnPropertyChanged(nameof(FechaDesde)); FiltrarCompras(); }
        }

        public DateTime FechaHasta
        {
            get => _fechaHasta;
            set { _fechaHasta = value; OnPropertyChanged(nameof(FechaHasta)); FiltrarCompras(); }
        }

        public ObservableCollection<Compra> Compras { get; set; } = new();
        public ObservableCollection<CompraDetalle> CompraDetalles { get; set; } = new();

        public Compra? CompraSeleccionada
        {
            get => _compraSeleccionada;
            set
            {
                _compraSeleccionada = value;
                OnPropertyChanged(nameof(CompraSeleccionada));
                CargarDetalleCompra();
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

        public GestionarComprasViewModel()
        {
            ComandoVerDetalle = new RelayCommand(_ => MostrarDetalle = true, _ => CompraSeleccionada != null);
            ComandoAnular = new RelayCommand(_ => AnularCompra(), _ => CompraSeleccionada != null && CompraSeleccionada.Estado == "Completada");
            ComandoCerrarDetalle = new RelayCommand(_ => MostrarDetalle = false);

            FiltrarCompras();
        }

        private void FiltrarCompras()
        {
            using var context = new Data.AppDbContext();
            var query = context.Compras
                .Where(c => c.Fecha >= FechaDesde && c.Fecha <= FechaHasta.AddDays(1))
                .OrderByDescending(c => c.Fecha);

            if (!string.IsNullOrWhiteSpace(Busqueda))
            {
                var busquedaLower = Busqueda.ToLower();
                query = (IOrderedQueryable<Compra>)query.Where(c =>
                    (c.Proveedor != null && c.Proveedor.Nombre.ToLower().Contains(busquedaLower)) ||
                    c.Id.ToString().Contains(Busqueda)
                );
            }

            var lista = query.ToList();
            Compras.Clear();
            foreach (var c in lista) Compras.Add(c);
        }

        private void CargarDetalleCompra()
        {
            CompraDetalles.Clear();
            if (CompraSeleccionada == null) return;

            using var context = new Data.AppDbContext();
            var detalles = context.CompraDetalles
                .Where(d => d.CompraId == CompraSeleccionada.Id)
                .ToList();

            foreach (var d in detalles) CompraDetalles.Add(d);
        }

        private void AnularCompra()
        {
            if (CompraSeleccionada == null) return;

            if (MessageBox.Show($"¿Anular la compra #{CompraSeleccionada.Id} por {CompraSeleccionada.Total:C}?", "Confirmar", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                using var context = new Data.AppDbContext();
                using var transaction = context.Database.BeginTransaction();

                try
                {
                    var compra = context.Compras.Find(CompraSeleccionada.Id);
                    if (compra != null)
                    {
                        compra.Estado = "Anulada";

                        // Revertir stock
                        var detalles = context.CompraDetalles.Where(d => d.CompraId == compra.Id).ToList();
                        foreach (var detalle in detalles)
                        {
                            var stock = context.Stocks.FirstOrDefault(s => s.ProductoId == detalle.ProductoId && s.AlmacenId == compra.AlmacenId);
                            if (stock != null)
                            {
                                stock.Cantidad -= detalle.Cantidad;
                            }
                        }

                        context.SaveChanges();
                        transaction.Commit();

                        MessageBox.Show("Compra anulada correctamente. El stock ha sido revertido.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                        FiltrarCompras();
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
