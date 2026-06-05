using LibriGest.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using BC = BCrypt.Net.BCrypt;

namespace LibriGest.ViewModels
{
    public class UsuariosViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private ObservableCollection<Usuario> _usuarios = new();
        private Usuario? _usuarioSeleccionado;
        private bool _mostrarFormulario;
        private bool _esEdicion;

        public ObservableCollection<Usuario> Usuarios
        {
            get => _usuarios;
            set { _usuarios = value; OnPropertyChanged(nameof(Usuarios)); }
        }

        public Usuario? UsuarioSeleccionado
        {
            get => _usuarioSeleccionado;
            set { _usuarioSeleccionado = value; OnPropertyChanged(nameof(UsuarioSeleccionado)); }
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

        public string FormNombreUsuario { get; set; } = "";
        public string FormNombreCompleto { get; set; } = "";
        public string FormClave { get; set; } = "";
        public string FormRol { get; set; } = "Vendedor";
        public bool FormActivo { get; set; } = true;

        public ICommand ComandoNuevo { get; }
        public ICommand ComandoEditar { get; }
        public ICommand ComandoGuardar { get; }
        public ICommand ComandoCancelar { get; }
        public ICommand ComandoEliminar { get; }

        public UsuariosViewModel()
        {
            ComandoNuevo = new RelayCommand(_ => NuevoUsuario());
            ComandoEditar = new RelayCommand(_ => EditarUsuario(), _ => UsuarioSeleccionado != null);
            ComandoGuardar = new RelayCommand(_ => GuardarUsuario());
            ComandoCancelar = new RelayCommand(_ => Cancelar());
            ComandoEliminar = new RelayCommand(_ => EliminarUsuario(), _ => UsuarioSeleccionado != null);

            CargarDatos();
        }

        private void CargarDatos()
        {
            using var context = new Data.AppDbContext();
            var lista = context.Usuarios.OrderBy(u => u.NombreCompleto).ToList();
            Usuarios.Clear();
            foreach (var u in lista) Usuarios.Add(u);
        }

        private void NuevoUsuario()
        {
            EsEdicion = false;
            FormNombreUsuario = "";
            FormNombreCompleto = "";
            FormClave = "";
            FormRol = "Vendedor";
            FormActivo = true;
            UsuarioSeleccionado = null;
            MostrarFormulario = true;
            OnPropertyChanged(string.Empty);
        }

        private void EditarUsuario()
        {
            if (UsuarioSeleccionado == null) return;
            EsEdicion = true;
            FormNombreUsuario = UsuarioSeleccionado.NombreUsuario;
            FormNombreCompleto = UsuarioSeleccionado.NombreCompleto;
            FormClave = ""; // No mostrar hash
            FormRol = UsuarioSeleccionado.Rol;
            FormActivo = UsuarioSeleccionado.Activo;
            MostrarFormulario = true;
            OnPropertyChanged(string.Empty);
        }

        private void GuardarUsuario()
        {
            if (string.IsNullOrWhiteSpace(FormNombreUsuario) || string.IsNullOrWhiteSpace(FormNombreCompleto))
            {
                MessageBox.Show("Nombre de usuario y nombre completo son obligatorios.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using var context = new Data.AppDbContext();

            if (EsEdicion && UsuarioSeleccionado != null)
            {
                var usuario = context.Usuarios.Find(UsuarioSeleccionado.Id);
                if (usuario != null)
                {
                    usuario.NombreUsuario = FormNombreUsuario;
                    usuario.NombreCompleto = FormNombreCompleto;
                    if (!string.IsNullOrWhiteSpace(FormClave))
                        usuario.ClaveHash = BC.HashPassword(FormClave);
                    usuario.Rol = FormRol;
                    usuario.Activo = FormActivo;
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(FormClave))
                {
                    MessageBox.Show("La contraseña es obligatoria para nuevos usuarios.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                context.Usuarios.Add(new Usuario
                {
                    NombreUsuario = FormNombreUsuario,
                    NombreCompleto = FormNombreCompleto,
                    ClaveHash = BC.HashPassword(FormClave),
                    Rol = FormRol,
                    Activo = FormActivo
                });
            }

            context.SaveChanges();
            MostrarFormulario = false;
            CargarDatos();
            MessageBox.Show(EsEdicion ? "Usuario actualizado." : "Usuario registrado.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Cancelar()
        {
            MostrarFormulario = false;
            UsuarioSeleccionado = null;
        }

        private void EliminarUsuario()
        {
            if (UsuarioSeleccionado == null) return;
            if (MessageBox.Show($"¿Eliminar a '{UsuarioSeleccionado.NombreUsuario}'?", "Confirmar", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                using var context = new Data.AppDbContext();
                var usuario = context.Usuarios.Find(UsuarioSeleccionado.Id);
                if (usuario != null)
                {
                    usuario.Activo = false;
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
