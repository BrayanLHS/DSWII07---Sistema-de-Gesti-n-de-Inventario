using SistemaInventario.Models;

namespace SistemaInventario.Data.Interfaces
{
    public interface IProveedorRepositorio
    {
        List<ProveedorViewModel> Listar();
        ProveedorViewModel? ObtenerPorId(int id);
        void Insertar(ProveedorViewModel proveedor);
        void Actualizar(ProveedorViewModel proveedor);
        void Eliminar(int id);
    }
}
