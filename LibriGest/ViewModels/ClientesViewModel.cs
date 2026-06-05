using LibriGest.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace LibriGest.ViewModels
{
    public class ClientesViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private string _busqueda = "";
        private Cliente? _clienteSeleccionado;
        private bool _mostrarFormulario;
        private bool _esEdicion;

        public string Busqueda
        {
            get => _busqueda;
            set { _busqueda = value; OnPropertyChanged(nameof(Busqueda)); FiltrarClientes(); }
        }

        public ObservableCollection<Cliente> Clientes { get; set; } = new();

        public Cliente? ClienteSeleccionado
        {
            get => _clienteSeleccionado;
            set { _clienteSeleccionado = value; OnPropertyChanged(nameof(ClienteSeleccionado)); }
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

        // Campos formulario
        public string FormNombre { get; set; } = "";
        public string? FormDocumento { get; set; }
        public string? FormTelefono { get; set; }
        public string? FormDireccion { get; set; }
        public string? FormEmail { get; set; }

        public ICommand ComandoNuevo { get; }
        public ICommand ComandoEditar { get; }
        public ICommand ComandoGuardar { get; }
        public ICommand ComandoCancelar { get; }
        public ICommand ComandoEliminar { get; }

        public ClientesViewModel()
        {
            ComandoNuevo = new RelayCommand(_ => NuevoCliente());
            ComandoEditar = new RelayCommand(_ => EditarCliente(), _ => ClienteSeleccionado != null);
            ComandoGuardar = new RelayCommand(_ => GuardarCliente());
            ComandoCancelar = new RelayCommand(_ => Cancelar());
            ComandoEliminar = new RelayCommand(_ => EliminarCliente(), _ => ClienteSeleccionado != null);

            CargarDatos();
        }

        private void CargarDatos()
        {
            using var context = new Data.AppDbContext();
            var lista = context.Clientes.OrderBy(c => c.Nombre).ToList();
            Clientes.Clear();
            foreach (var c in lista) Clientes.Add(c);
        }

        private void FiltrarClientes()
        {
            using var context = new Data.AppDbContext();
            var query = context.Clientes.AsQueryable();

            if (!string.IsNullOrWhiteSpace(Busqueda))
            {
                var busquedaLower = Busqueda.ToLower();
                query = query.Where(c => c.Nombre.ToLower().Contains(busquedaLower) || 
                                    (c.Documento != null && c.Documento.Contains(Busqueda)));
            }

            var lista = query.OrderBy(c => c.Nombre).ToList();
            Clientes.Clear();
            foreach (var c in lista) Clientes.Add(c);
        }

        private void NuevoCliente()
        {
            EsEdicion = false;
            FormNombre = "";
            FormDocumento = null;
            FormTelefono = null;
            FormDireccion = null;
            FormEmail = null;
            ClienteSeleccionado = null;
            MostrarFormulario = true;
            OnPropertyChanged(string.Empty);
        }

        private void EditarCliente()
        {
            if (ClienteSeleccionado == null) return;
            EsEdicion = true;
            FormNombre = ClienteSeleccionado.Nombre;
            FormDocumento = ClienteSeleccionado.Documento;
            FormTelefono = ClienteSeleccionado.Telefono;
            FormDireccion = ClienteSeleccionado.Direccion;
            FormEmail = ClienteSeleccionado.Email;
            MostrarFormulario = true;
            OnPropertyChanged(string.Empty);
        }

        private void GuardarCliente()
        {
            if (string.IsNullOrWhiteSpace(FormNombre))
            {
                MessageBox.Show("El nombre es obligatorio.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using var context = new Data.AppDbContext();

            if (EsEdicion && ClienteSeleccionado != null)
            {
                var cliente = context.Clientes.Find(ClienteSeleccionado.Id);
                if (cliente != null)
                {
                    cliente.Nombre = FormNombre;
                    cliente.Documento = FormDocumento;
                    cliente.Telefono = FormTelefono;
                    cliente.Direccion = FormDireccion;
                    cliente.Email = FormEmail;
                }
            }
            else
            {
                context.Clientes.Add(new Cliente
                {
                    Nombre = FormNombre,
                    Documento = FormDocumento,
                    Telefono = FormTelefono,
                    Direccion = FormDireccion,
                    Email = FormEmail
                });
            }

            context.SaveChanges();
            MostrarFormulario = false;
            CargarDatos();
            MessageBox.Show(EsEdicion ? "Cliente actualizado." : "Cliente registrado.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Cancelar()
        {
            MostrarFormulario = false;
            ClienteSeleccionado = null;
        }

        private void EliminarCliente()
        {
            if (ClienteSeleccionado == null) return;
            if (MessageBox.Show($"¿Eliminar a '{ClienteSeleccionado.Nombre}'?", "Confirmar", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                using var context = new Data.AppDbContext();
                var cliente = context.Clientes.Find(ClienteSeleccionado.Id);
                if (cliente != null)
                {
                    context.Clientes.Remove(cliente);
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
