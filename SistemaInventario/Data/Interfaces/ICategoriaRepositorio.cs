using SistemaInventario.Models;

namespace SistemaInventario.Data.Interfaces
{
    public interface ICategoriaRepositorio
    {
        List<CategoriaViewModel> Listar(string? buscar = null);
        CategoriaViewModel? ObtenerPorId(int id);
        void Insertar(CategoriaViewModel categoria);
        void Actualizar(CategoriaViewModel categoria);
        void Eliminar(int id);
        bool TieneProductosAsociados(int id);
    }
}
