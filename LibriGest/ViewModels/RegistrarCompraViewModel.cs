using LibriGest.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace LibriGest.ViewModels
{
    public class ItemCompra
    {
        public int ProductoId { get; set; }
        public string? CodigoBarras { get; set; }
        public string Nombre { get; set; } = "";
        public int Cantidad { get; set; } = 1;
        public decimal PrecioUnitario { get; set; }
        public decimal Total => Cantidad * PrecioUnitario;
    }

    public class RegistrarCompraViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private string _inputCodigoBarras = "";
        private ObservableCollection<Proveedor> _proveedores = new();
        private Proveedor? _proveedorSeleccionado;
        private Almacen? _almacenSeleccionado;
        private ObservableCollection<Almacen> _almacenes = new();

        public string InputCodigoBarras
        {
            get => _inputCodigoBarras;
            set { _inputCodigoBarras = value; OnPropertyChanged(nameof(InputCodigoBarras)); }
        }

        public ObservableCollection<ItemCompra> Items { get; set; } = new();
        public ObservableCollection<Proveedor> Proveedores
        {
            get => _proveedores;
            set { _proveedores = value; OnPropertyChanged(nameof(Proveedores)); }
        }

        public Proveedor? ProveedorSeleccionado
        {
            get => _proveedorSeleccionado;
            set { _proveedorSeleccionado = value; OnPropertyChanged(nameof(ProveedorSeleccionado)); }
        }

        public ObservableCollection<Almacen> Almacenes
        {
            get => _almacenes;
            set { _almacenes = value; OnPropertyChanged(nameof(Almacenes)); }
        }

        public Almacen? AlmacenSeleccionado
        {
            get => _almacenSeleccionado;
            set { _almacenSeleccionado = value; OnPropertyChanged(nameof(AlmacenSeleccionado)); }
        }

        public decimal Total => Items.Sum(i => i.Total);

        public ICommand ComandoBuscarProducto { get; }
        public ICommand ComandoEliminarItem { get; }
        public ICommand ComandoCompletarCompra { get; }
        public ICommand ComandoCancelarCompra { get; }

        public RegistrarCompraViewModel()
        {
            ComandoBuscarProducto = new RelayCommand(_ => BuscarProductoPorCodigo());
            ComandoEliminarItem = new RelayCommand(param => EliminarItem((ItemCompra)param!));
            ComandoCompletarCompra = new RelayCommand(_ => CompletarCompra(), _ => Items.Any() && ProveedorSeleccionado != null);
            ComandoCancelarCompra = new RelayCommand(_ => CancelarCompra());

            CargarDatos();
        }

        private void CargarDatos()
        {
            using var context = new Data.AppDbContext();
            var provs = context.Proveedores.OrderBy(p => p.Nombre).ToList();
            Proveedores.Clear();
            foreach (var p in provs) Proveedores.Add(p);
            ProveedorSeleccionado = provs.FirstOrDefault();

            var alms = context.Almacenes.ToList();
            Almacenes.Clear();
            foreach (var a in alms) Almacenes.Add(a);
            AlmacenSeleccionado = alms.FirstOrDefault();
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
                    Items.Add(new ItemCompra
                    {
                        ProductoId = producto.Id,
                        CodigoBarras = producto.CodigoBarras,
                        Nombre = producto.Nombre,
                        PrecioUnitario = producto.PrecioCompra
                    });
                }

                OnPropertyChanged(nameof(Items));
                OnPropertyChanged(nameof(Total));
            }
            else
            {
                MessageBox.Show($"Producto con código '{InputCodigoBarras}' no encontrado.", "No encontrado", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            InputCodigoBarras = "";
            OnPropertyChanged(nameof(InputCodigoBarras));
        }

        private void EliminarItem(ItemCompra item)
        {
            Items.Remove(item);
            OnPropertyChanged(nameof(Total));
        }

        private void CompletarCompra()
        {
            if (!Items.Any() || ProveedorSeleccionado == null || AlmacenSeleccionado == null) return;

            using var context = new Data.AppDbContext();
            using var transaction = context.Database.BeginTransaction();

            try
            {
                var compra = new Compra
                {
                    Fecha = DateTime.Now,
                    ProveedorId = ProveedorSeleccionado.Id,
                    UsuarioId = 1,
                    Total = Total,
                    Estado = "Completada",
                    AlmacenId = AlmacenSeleccionado.Id
                };

                context.Compras.Add(compra);
                context.SaveChanges();

                foreach (var item in Items)
                {
                    context.CompraDetalles.Add(new CompraDetalle
                    {
                        CompraId = compra.Id,
                        ProductoId = item.ProductoId,
                        Cantidad = item.Cantidad,
                        PrecioUnitario = item.PrecioUnitario,
                        Total = item.Total
                    });

                    // Aumentar stock
                    var stock = context.Stocks.FirstOrDefault(s => s.ProductoId == item.ProductoId && s.AlmacenId == AlmacenSeleccionado.Id);
                    if (stock != null)
                    {
                        stock.Cantidad += item.Cantidad;
                    }
                    else
                    {
                        context.Stocks.Add(new Stock
                        {
                            ProductoId = item.ProductoId,
                            AlmacenId = AlmacenSeleccionado.Id,
                            Cantidad = item.Cantidad
                        });
                    }
                }

                context.SaveChanges();
                transaction.Commit();

                MessageBox.Show("Compra registrada exitosamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                CancelarCompra();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                MessageBox.Show($"Error al registrar la compra: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelarCompra()
        {
            Items.Clear();
            ProveedorSeleccionado = Proveedores.FirstOrDefault();
            AlmacenSeleccionado = Almacenes.FirstOrDefault();
            InputCodigoBarras = "";
            OnPropertyChanged(string.Empty);
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
