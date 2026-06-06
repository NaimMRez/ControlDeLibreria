using LibriGest.Helpers;
using System.Collections.Generic;
using System.Linq;

namespace LibriGest.Helpers
{
    public static class Authorization
    {
        // Simple role -> permissions mapping
        private static readonly Dictionary<string, string[]> RolePermissions = new()
        {
            ["Administrador"] = new[] { "*" },
            ["Vendedor"] = new[] { "RegistrarVenta", "GestionarVentas", "Clientes", "Dashboard" },
            ["Almacenero"] = new[] { "RegistrarCompra", "GestionarCompras", "Productos", "AjustesInventario", "Dashboard" }
        };

        public static bool HasPermission(string permission)
        {
            var user = SessionContext.CurrentUser;
            if (user == null) return false;

            if (RolePermissions.TryGetValue(user.Rol ?? string.Empty, out var perms))
            {
                if (perms.Contains("*")) return true;
                return perms.Contains(permission);
            }

            return false;
        }
    }
}
