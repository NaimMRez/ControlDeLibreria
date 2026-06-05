using LibriGest.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;

namespace LibriGest.ViewModels
{
    public class DashboardViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private int _totalProductos;
        private int _productosStockBajo;
        private decimal _ventasHoy;
        private int _totalClientes;
        private int _totalProveedores;
        private string _estadoCaja = "Cerrada";
        private decimal _montoCaja;

        public int TotalProductos
        {
            get => _totalProductos;
            set { _totalProductos = value; OnPropertyChanged(nameof(TotalProductos)); }
        }

        public int ProductosStockBajo
        {
            get => _productosStockBajo;
            set { _productosStockBajo = value; OnPropertyChanged(nameof(ProductosStockBajo)); }
        }

        public decimal VentasHoy
        {
            get => _ventasHoy;
            set { _ventasHoy = value; OnPropertyChanged(nameof(VentasHoy)); }
        }

        public int TotalClientes
        {
            get => _totalClientes;
            set { _totalClientes = value; OnPropertyChanged(nameof(TotalClientes)); }
        }

        public int TotalProveedores
        {
            get => _totalProveedores;
            set { _totalProveedores = value; OnPropertyChanged(nameof(TotalProveedores)); }
        }

        public string EstadoCaja
        {
            get => _estadoCaja;
            set { _estadoCaja = value; OnPropertyChanged(nameof(EstadoCaja)); }
        }

        public decimal MontoCaja
        {
            get => _montoCaja;
            set { _montoCaja = value; OnPropertyChanged(nameof(MontoCaja)); }
        }

        public DashboardViewModel()
        {
            CargarEstadisticas();
        }

        private void CargarEstadisticas()
        {
            using var context = new Data.AppDbContext();

            TotalProductos = context.Productos.Count(p => p.Activo);
            
            // Productos con stock bajo
            var stocks = context.Stocks.Include(s => s.Producto).ToList();
            ProductosStockBajo = stocks.Count(s => s.Producto != null && s.Cantidad <= s.Producto.StockMinimo);

            // Ventas de hoy
            var hoy = DateTime.Now.Date;
            VentasHoy = context.Ventas
                .Where(v => v.Fecha >= hoy && v.Estado == "Completada")
                .Sum(v => (decimal?)v.Total) ?? 0;

            TotalClientes = context.Clientes.Count();
            TotalProveedores = context.Proveedores.Count();

            // Estado de caja
            var caja = context.Cajas.FirstOrDefault(c => c.Estado == "Abierta");
            if (caja != null)
            {
                EstadoCaja = "Abierta";
                var ventasCaja = context.Ventas.Where(v => v.CajaId == caja.Id && v.Estado == "Completada").Sum(v => (decimal?)v.Total) ?? 0;
                var gastosCaja = context.Gastos.Where(g => g.CajaId == caja.Id).Sum(g => (decimal?)g.Monto) ?? 0;
                MontoCaja = caja.MontoInicial + ventasCaja - gastosCaja;
            }
            else
            {
                EstadoCaja = "Cerrada";
                MontoCaja = 0;
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
