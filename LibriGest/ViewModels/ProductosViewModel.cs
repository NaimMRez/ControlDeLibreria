using LibriGest.Models;
using LibriGest.ViewModels;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace LibriGest.ViewModels
{
    public class ProductosViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private string _busqueda = "";
        private Producto? _productoSeleccionado;
        private bool _mostrarFormulario;
        private bool _esEdicion;

        public string Busqueda
        {
            get => _busqueda;
            set { _busqueda = value; OnPropertyChanged(nameof(Busqueda)); FiltrarProductos(); }
        }

        public ObservableCollection<Producto> Productos { get; set; } = new();
        public ObservableCollection<Categoria> Categorias { get; set; } = new();
        public ObservableCollection<Unidad> Unidades { get; set; } = new();

        public Producto? ProductoSeleccionado
        {
            get => _productoSeleccionado;
            set { _productoSeleccionado = value; OnPropertyChanged(nameof(ProductoSeleccionado)); }
        }

        public bool MostrarFormulario
        {
            get => _mostrarFormulario;
            set { _mostrarFormulario = value; OnPropertyChanged(nameof(MostrarFormulario)); }
        }

        public bool EsEdicion
        {
            get => _esEdicion;
            set { _esEdicion = value; OnPropertyChanged(nameof(EsEdicion)); }
        }

        // Campos del formulario
        public string FormCodigoBarras { get; set; } = "";
        public string FormNombre { get; set; } = "";
        public string FormDescripcion { get; set; } = "";
        public int FormCategoriaId { get; set; }
        public int FormUnidadId { get; set; }
        public decimal FormPrecioCompra { get; set; }
        public decimal FormPrecioVenta { get; set; }
        public int FormStockMinimo { get; set; }

        public ICommand ComandoNuevo { get; }
        public ICommand ComandoEditar { get; }
        public ICommand ComandoGuardar { get; }
        public ICommand ComandoCancelar { get; }
        public ICommand ComandoEliminar { get; }

        public ProductosViewModel()
        {
            ComandoNuevo = new RelayCommand(_ => NuevoProducto());
            ComandoEditar = new RelayCommand(_ => EditarProducto(), _ => ProductoSeleccionado != null);
            ComandoGuardar = new RelayCommand(_ => GuardarProducto());
            ComandoCancelar = new RelayCommand(_ => Cancelar());
            ComandoEliminar = new RelayCommand(_ => EliminarProducto(), _ => ProductoSeleccionado != null);

            CargarDatos();
        }

        private void CargarDatos()
        {
            using var context = new Data.AppDbContext();
            var productos = context.Productos.Where(p => p.Activo).ToList();
            Productos.Clear();
            foreach (var p in productos) Productos.Add(p);

            Categorias.Clear();
            foreach (var c in context.Categorias.ToList()) Categorias.Add(c);

            Unidades.Clear();
            foreach (var u in context.Unidades.ToList()) Unidades.Add(u);
        }

        private void FiltrarProductos()
        {
            using var context = new Data.AppDbContext();
            var query = context.Productos.Where(p => p.Activo);

            if (!string.IsNullOrWhiteSpace(Busqueda))
            {
                var busquedaLower = Busqueda.ToLower();
                query = query.Where(p => 
                    (p.Nombre != null && p.Nombre.ToLower().Contains(busquedaLower)) ||
                    (p.CodigoBarras != null && p.CodigoBarras.ToLower().Contains(busquedaLower))
                );
            }

            var lista = query.ToList();
            Productos.Clear();
            foreach (var p in lista) Productos.Add(p);
        }

        private void NuevoProducto()
        {
            EsEdicion = false;
            FormCodigoBarras = "";
            FormNombre = "";
            FormDescripcion = "";
            FormCategoriaId = Categorias.FirstOrDefault()?.Id ?? 0;
            FormUnidadId = Unidades.FirstOrDefault()?.Id ?? 0;
            FormPrecioCompra = 0;
            FormPrecioVenta = 0;
            FormStockMinimo = 0;
            ProductoSeleccionado = null;
            MostrarFormulario = true;
            OnPropertyChanged(string.Empty); // Refrescar todos
        }

        private void EditarProducto()
        {
            if (ProductoSeleccionado == null) return;
            EsEdicion = true;
            FormCodigoBarras = ProductoSeleccionado.CodigoBarras ?? "";
            FormNombre = ProductoSeleccionado.Nombre;
            FormDescripcion = ProductoSeleccionado.Descripcion ?? "";
            FormCategoriaId = ProductoSeleccionado.CategoriaId;
            FormUnidadId = ProductoSeleccionado.UnidadId;
            FormPrecioCompra = ProductoSeleccionado.PrecioCompra;
            FormPrecioVenta = ProductoSeleccionado.PrecioVenta;
            FormStockMinimo = ProductoSeleccionado.StockMinimo;
            MostrarFormulario = true;
            OnPropertyChanged(string.Empty);
        }

        private void GuardarProducto()
        {
            if (string.IsNullOrWhiteSpace(FormNombre))
            {
                MessageBox.Show("El nombre del producto es obligatorio.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using var context = new Data.AppDbContext();

            if (EsEdicion && ProductoSeleccionado != null)
            {
                var producto = context.Productos.Find(ProductoSeleccionado.Id);
                if (producto != null)
                {
                    producto.CodigoBarras = FormCodigoBarras;
                    producto.Nombre = FormNombre;
                    producto.Descripcion = FormDescripcion;
                    producto.CategoriaId = FormCategoriaId;
                    producto.UnidadId = FormUnidadId;
                    producto.PrecioCompra = FormPrecioCompra;
                    producto.PrecioVenta = FormPrecioVenta;
                    producto.StockMinimo = FormStockMinimo;
                }
            }
            else
            {
                var nuevo = new Producto
                {
                    CodigoBarras = FormCodigoBarras,
                    Nombre = FormNombre,
                    Descripcion = FormDescripcion,
                    CategoriaId = FormCategoriaId,
                    UnidadId = FormUnidadId,
                    PrecioCompra = FormPrecioCompra,
                    PrecioVenta = FormPrecioVenta,
                    StockMinimo = FormStockMinimo,
                    Activo = true
                };
                context.Productos.Add(nuevo);

                // Crear stock inicial en almacén principal
                var almacenPrincipal = context.Almacenes.FirstOrDefault();
                if (almacenPrincipal != null)
                {
                    context.Stocks.Add(new Stock
                    {
                        ProductoId = nuevo.Id,
                        AlmacenId = almacenPrincipal.Id,
                        Cantidad = 0
                    });
                }
            }

            context.SaveChanges();
            MostrarFormulario = false;
            CargarDatos();
            MessageBox.Show(EsEdicion ? "Producto actualizado." : "Producto registrado.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Cancelar()
        {
            MostrarFormulario = false;
            ProductoSeleccionado = null;
        }

        private void EliminarProducto()
        {
            if (ProductoSeleccionado == null) return;

            if (MessageBox.Show($"¿Eliminar el producto '{ProductoSeleccionado.Nombre}'?", "Confirmar", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                using var context = new Data.AppDbContext();
                var producto = context.Productos.Find(ProductoSeleccionado.Id);
                if (producto != null)
                {
                    producto.Activo = false;
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
