using LibriGest.Models;
using LibriGest.ViewModels;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace LibriGest.ViewModels
{
    public class ItemVenta
    {
        public int ProductoId { get; set; }
        public string? CodigoBarras { get; set; }
        public string Nombre { get; set; } = "";
        public int Cantidad { get; set; } = 1;
        public decimal PrecioUnitario { get; set; }
        public decimal Total => Cantidad * PrecioUnitario;
    }

    public class RegistrarVentaViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private string _inputCodigoBarras = "";
        private ObservableCollection<Cliente> _clientes = new();
        private Cliente? _clienteSeleccionado;
        private decimal _descuento = 0;

        public string InputCodigoBarras
        {
            get => _inputCodigoBarras;
            set { _inputCodigoBarras = value; OnPropertyChanged(nameof(InputCodigoBarras)); }
        }

        public ObservableCollection<ItemVenta> Items { get; set; } = new();
        public ObservableCollection<Cliente> Clientes
        {
            get => _clientes;
            set { _clientes = value; OnPropertyChanged(nameof(Clientes)); }
        }

        public Cliente? ClienteSeleccionado
        {
            get => _clienteSeleccionado;
            set { _clienteSeleccionado = value; OnPropertyChanged(nameof(ClienteSeleccionado)); }
        }

        public decimal SubTotal => Items.Sum(i => i.Total);
        public decimal Descuento
        {
            get => _descuento;
            set { _descuento = value; OnPropertyChanged(nameof(Descuento)); OnPropertyChanged(nameof(Total)); }
        }
        public decimal Total => SubTotal - Descuento;

        public ICommand ComandoBuscarProducto { get; }
        public ICommand ComandoEliminarItem { get; }
        public ICommand ComandoCompletarVenta { get; }
        public ICommand ComandoCancelarVenta { get; }

        public RegistrarVentaViewModel()
        {
            ComandoBuscarProducto = new RelayCommand(_ => BuscarProductoPorCodigo());
            ComandoEliminarItem = new RelayCommand(param => EliminarItem((ItemVenta)param!));
            ComandoCompletarVenta = new RelayCommand(_ => CompletarVenta(), _ => Items.Any());
            ComandoCancelarVenta = new RelayCommand(_ => CancelarVenta());

            CargarClientes();
        }

        private void CargarClientes()
        {
            using var context = new Data.AppDbContext();
            var lista = context.Clientes.OrderBy(c => c.Nombre).ToList();
            Clientes.Clear();
            foreach (var c in lista) Clientes.Add(c);
        }

        public void BuscarProductoPorCodigo()
        {
            if (string.IsNullOrWhiteSpace(InputCodigoBarras)) return;

            using var context = new Data.AppDbContext();
            var producto = context.Productos
                .FirstOrDefault(p => p.CodigoBarras == InputCodigoBarras && p.Activo);

            if (producto != null)
            {
                var itemExistente = Items.FirstOrDefault(i => i.ProductoId == producto.Id);
                if (itemExistente != null)
                {
                    itemExistente.Cantidad++;
                }
                else
                {
                    Items.Add(new ItemVenta
                    {
                        ProductoId = producto.Id,
                        CodigoBarras = producto.CodigoBarras,
                        Nombre = producto.Nombre,
                        PrecioUnitario = producto.PrecioVenta
                    });
                }

                // Refrescar totales
                OnPropertyChanged(nameof(Items));
                OnPropertyChanged(nameof(SubTotal));
                OnPropertyChanged(nameof(Total));
            }
            else
            {
                MessageBox.Show($"Producto con código '{InputCodigoBarras}' no encontrado.", "No encontrado", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            InputCodigoBarras = "";
            OnPropertyChanged(nameof(InputCodigoBarras));
        }

        private void EliminarItem(ItemVenta item)
        {
            Items.Remove(item);
            OnPropertyChanged(nameof(SubTotal));
            OnPropertyChanged(nameof(Total));
        }

        private void CompletarVenta()
        {
            if (!Items.Any()) return;

            using var context = new Data.AppDbContext();
            using var transaction = context.Database.BeginTransaction();

            try
            {
                var venta = new Venta
                {
                    Fecha = DateTime.Now,
                    ClienteId = ClienteSeleccionado?.Id,
                    UsuarioId = 1, // TODO: Usuario actual
                    SubTotal = SubTotal,
                    Descuento = Descuento,
                    Total = Total,
                    Estado = "Completada"
                };

                context.Ventas.Add(venta);
                context.SaveChanges(); // Para obtener el Id de la venta

                foreach (var item in Items)
                {
                    context.VentaDetalles.Add(new VentaDetalle
                    {
                        VentaId = venta.Id,
                        ProductoId = item.ProductoId,
                        Cantidad = item.Cantidad,
                        PrecioUnitario = item.PrecioUnitario,
                        Total = item.Total
                    });

                    // Descontar stock
                    var stock = context.Stocks.FirstOrDefault(s => s.ProductoId == item.ProductoId);
                    if (stock != null)
                    {
                        stock.Cantidad -= item.Cantidad;
                    }
                }

                context.SaveChanges();
                transaction.Commit();

                MessageBox.Show("Venta registrada exitosamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                CancelarVenta();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                MessageBox.Show($"Error al registrar la venta: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelarVenta()
        {
            Items.Clear();
            Descuento = 0;
            ClienteSeleccionado = null;
            InputCodigoBarras = "";
            OnPropertyChanged(string.Empty);
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
