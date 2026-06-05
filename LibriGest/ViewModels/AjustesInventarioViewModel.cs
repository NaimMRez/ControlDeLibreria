using LibriGest.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace LibriGest.ViewModels
{
    public class ItemAjuste
    {
        public int ProductoId { get; set; }
        public string? CodigoBarras { get; set; }
        public string Nombre { get; set; } = "";
        public int CantidadAnterior { get; set; }
        public int CantidadNueva { get; set; }
        public int Diferencia => CantidadNueva - CantidadAnterior;
    }

    public class AjustesInventarioViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private string _inputCodigoBarras = "";
        private ObservableCollection<Almacen> _almacenes = new();
        private Almacen? _almacenSeleccionado;
        private string _motivo = "";
        private string _tipo = "Entrada";

        public string InputCodigoBarras
        {
            get => _inputCodigoBarras;
            set { _inputCodigoBarras = value; OnPropertyChanged(nameof(InputCodigoBarras)); }
        }

        public ObservableCollection<ItemAjuste> Items { get; set; } = new();

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

        public string Motivo
        {
            get => _motivo;
            set { _motivo = value; OnPropertyChanged(nameof(Motivo)); }
        }

        public string Tipo
        {
            get => _tipo;
            set { _tipo = value; OnPropertyChanged(nameof(Tipo)); }
        }

        public ICommand ComandoBuscarProducto { get; }
        public ICommand ComandoEliminarItem { get; }
        public ICommand ComandoRegistrarAjuste { get; }
        public ICommand ComandoCancelar { get; }

        public AjustesInventarioViewModel()
        {
            ComandoBuscarProducto = new RelayCommand(_ => BuscarProducto());
            ComandoEliminarItem = new RelayCommand(param => EliminarItem((ItemAjuste)param!));
            ComandoRegistrarAjuste = new RelayCommand(_ => RegistrarAjuste(), _ => Items.Any());
            ComandoCancelar = new RelayCommand(_ => Cancelar());

            CargarAlmacenes();
        }

        private void CargarAlmacenes()
        {
            using var context = new Data.AppDbContext();
            var alms = context.Almacenes.ToList();
            Almacenes.Clear();
            foreach (var a in alms) Almacenes.Add(a);
            AlmacenSeleccionado = alms.FirstOrDefault();
        }

        public void BuscarProducto()
        {
            if (string.IsNullOrWhiteSpace(InputCodigoBarras) || AlmacenSeleccionado == null) return;

            using var context = new Data.AppDbContext();
            var producto = context.Productos
                .FirstOrDefault(p => p.CodigoBarras == InputCodigoBarras && p.Activo);

            if (producto != null)
            {
                var stock = context.Stocks.FirstOrDefault(s => s.ProductoId == producto.Id && s.AlmacenId == AlmacenSeleccionado.Id);
                int cantidadActual = stock?.Cantidad ?? 0;

                if (Items.Any(i => i.ProductoId == producto.Id))
                {
                    MessageBox.Show("Este producto ya está en la lista.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    Items.Add(new ItemAjuste
                    {
                        ProductoId = producto.Id,
                        CodigoBarras = producto.CodigoBarras,
                        Nombre = producto.Nombre,
                        CantidadAnterior = cantidadActual,
                        CantidadNueva = cantidadActual
                    });
                    OnPropertyChanged(nameof(Items));
                }
            }
            else
            {
                MessageBox.Show($"Producto con código '{InputCodigoBarras}' no encontrado.", "No encontrado", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            InputCodigoBarras = "";
            OnPropertyChanged(nameof(InputCodigoBarras));
        }

        private void EliminarItem(ItemAjuste item)
        {
            Items.Remove(item);
        }

        private void RegistrarAjuste()
        {
            if (!Items.Any() || AlmacenSeleccionado == null) return;

            if (string.IsNullOrWhiteSpace(Motivo))
            {
                MessageBox.Show("Debe ingresar un motivo para el ajuste.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using var context = new Data.AppDbContext();
            using var transaction = context.Database.BeginTransaction();

            try
            {
                var ajuste = new AjusteInventario
                {
                    Fecha = DateTime.Now,
                    AlmacenId = AlmacenSeleccionado.Id,
                    UsuarioId = 1,
                    Tipo = Tipo,
                    Motivo = Motivo
                };

                context.AjustesInventario.Add(ajuste);
                context.SaveChanges();

                foreach (var item in Items)
                {
                    context.AjusteDetalles.Add(new AjusteDetalle
                    {
                        AjusteInventarioId = ajuste.Id,
                        ProductoId = item.ProductoId,
                        CantidadAnterior = item.CantidadAnterior,
                        CantidadNueva = item.CantidadNueva,
                        Diferencia = item.Diferencia
                    });

                    // Actualizar stock
                    var stock = context.Stocks.FirstOrDefault(s => s.ProductoId == item.ProductoId && s.AlmacenId == AlmacenSeleccionado.Id);
                    if (stock != null)
                    {
                        stock.Cantidad = item.CantidadNueva;
                    }
                    else
                    {
                        context.Stocks.Add(new Stock
                        {
                            ProductoId = item.ProductoId,
                            AlmacenId = AlmacenSeleccionado.Id,
                            Cantidad = item.CantidadNueva
                        });
                    }
                }

                context.SaveChanges();
                transaction.Commit();

                MessageBox.Show("Ajuste de inventario registrado correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                Cancelar();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Cancelar()
        {
            Items.Clear();
            Motivo = "";
            InputCodigoBarras = "";
            OnPropertyChanged(string.Empty);
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
