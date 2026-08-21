using SistemaInventario.Models;

namespace SistemaInventario.Data.Interfaces
{
    public interface IProveedorRepositorio
    {
        List<ProveedorViewModel> Listar(string? buscar = null);
        ProveedorViewModel? ObtenerPorId(int id);
        int Insertar(ProveedorViewModel proveedor);
        void Actualizar(ProveedorViewModel proveedor);
        void Eliminar(int id);
    }
}
