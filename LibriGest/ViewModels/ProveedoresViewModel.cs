using LibriGest.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace LibriGest.ViewModels
{
    public class ProveedoresViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private string _busqueda = "";
        private Proveedor? _proveedorSeleccionado;
        private bool _mostrarFormulario;
        private bool _esEdicion;

        public string Busqueda
        {
            get => _busqueda;
            set { _busqueda = value; OnPropertyChanged(nameof(Busqueda)); FiltrarProveedores(); }
        }

        public ObservableCollection<Proveedor> Proveedores { get; set; } = new();

        public Proveedor? ProveedorSeleccionado
        {
            get => _proveedorSeleccionado;
            set { _proveedorSeleccionado = value; OnPropertyChanged(nameof(ProveedorSeleccionado)); }
        }

        public bool MostrarFormulario
        {
            get => _mostrarFormulario;
            set { _mostrarFormulario = value; OnPropertyChanged(nameof(MostrarFormulario)); OnPropertyChanged(nameof(MostrarLista)); }
        }

        public bool MostrarLista => !_mostrarFormulario;

        public bool EsEdicion
        {
            get => _esEdicion;
            set { _esEdicion = value; OnPropertyChanged(nameof(EsEdicion)); }
        }

        // Campos formulario
        public string FormNombre { get; set; } = "";
        public string? FormRUC { get; set; }
        public string? FormTelefono { get; set; }
        public string? FormDireccion { get; set; }
        public string? FormEmail { get; set; }

        public ICommand ComandoNuevo { get; }
        public ICommand ComandoEditar { get; }
        public ICommand ComandoGuardar { get; }
        public ICommand ComandoCancelar { get; }
        public ICommand ComandoEliminar { get; }

        public ProveedoresViewModel()
        {
            ComandoNuevo = new RelayCommand(_ => NuevoProveedor());
            ComandoEditar = new RelayCommand(_ => EditarProveedor(), _ => ProveedorSeleccionado != null);
            ComandoGuardar = new RelayCommand(_ => GuardarProveedor());
            ComandoCancelar = new RelayCommand(_ => Cancelar());
            ComandoEliminar = new RelayCommand(_ => EliminarProveedor(), _ => ProveedorSeleccionado != null);

            CargarDatos();
        }

        private void CargarDatos()
        {
            using var context = new Data.AppDbContext();
            var lista = context.Proveedores.OrderBy(p => p.Nombre).ToList();
            Proveedores.Clear();
            foreach (var p in lista) Proveedores.Add(p);
        }

        private void FiltrarProveedores()
        {
            using var context = new Data.AppDbContext();
            var query = context.Proveedores.AsQueryable();

            if (!string.IsNullOrWhiteSpace(Busqueda))
            {
                var busquedaLower = Busqueda.ToLower();
                query = query.Where(p => p.Nombre.ToLower().Contains(busquedaLower) ||
                                    (p.RUC != null && p.RUC.Contains(Busqueda)));
            }

            var lista = query.OrderBy(p => p.Nombre).ToList();
            Proveedores.Clear();
            foreach (var p in lista) Proveedores.Add(p);
        }

        private void NuevoProveedor()
        {
            EsEdicion = false;
            FormNombre = "";
            FormRUC = null;
            FormTelefono = null;
            FormDireccion = null;
            FormEmail = null;
            ProveedorSeleccionado = null;
            MostrarFormulario = true;
            OnPropertyChanged(string.Empty);
        }

        private void EditarProveedor()
        {
            if (ProveedorSeleccionado == null) return;
            EsEdicion = true;
            FormNombre = ProveedorSeleccionado.Nombre;
            FormRUC = ProveedorSeleccionado.RUC;
            FormTelefono = ProveedorSeleccionado.Telefono;
            FormDireccion = ProveedorSeleccionado.Direccion;
            FormEmail = ProveedorSeleccionado.Email;
            MostrarFormulario = true;
            OnPropertyChanged(string.Empty);
        }

        private void GuardarProveedor()
        {
            if (string.IsNullOrWhiteSpace(FormNombre))
            {
                MessageBox.Show("El nombre es obligatorio.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using var context = new Data.AppDbContext();

            if (EsEdicion && ProveedorSeleccionado != null)
            {
                var proveedor = context.Proveedores.Find(ProveedorSeleccionado.Id);
                if (proveedor != null)
                {
                    proveedor.Nombre = FormNombre;
                    proveedor.RUC = FormRUC;
                    proveedor.Telefono = FormTelefono;
                    proveedor.Direccion = FormDireccion;
                    proveedor.Email = FormEmail;
                }
            }
            else
            {
                context.Proveedores.Add(new Proveedor
                {
                    Nombre = FormNombre,
                    RUC = FormRUC,
                    Telefono = FormTelefono,
                    Direccion = FormDireccion,
                    Email = FormEmail
                });
            }

            context.SaveChanges();
            MostrarFormulario = false;
            CargarDatos();
            MessageBox.Show(EsEdicion ? "Proveedor actualizado." : "Proveedor registrado.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Cancelar()
        {
            MostrarFormulario = false;
            ProveedorSeleccionado = null;
        }

        private void EliminarProveedor()
        {
            if (ProveedorSeleccionado == null) return;
            if (MessageBox.Show($"¿Eliminar a '{ProveedorSeleccionado.Nombre}'?", "Confirmar", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                using var context = new Data.AppDbContext();
                var proveedor = context.Proveedores.Find(ProveedorSeleccionado.Id);
                if (proveedor != null)
                {
                    context.Proveedores.Remove(proveedor);
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
