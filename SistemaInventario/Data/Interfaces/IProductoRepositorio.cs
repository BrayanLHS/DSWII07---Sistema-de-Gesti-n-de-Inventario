using SistemaInventario.Models;

namespace SistemaInventario.Data.Interfaces
{
    public interface IProductoRepositorio
    {
        List<ProductoViewModel> Listar(string? buscar = null, int? idCategoria = null, int? idProveedor = null, int? stockMenorQue = null);
        List<ProductoViewModel> ListarPaginado(int pagina, int tamano, out int total);
        ProductoViewModel? ObtenerPorId(int id);
        int Insertar(ProductoViewModel producto);
        void Actualizar(ProductoViewModel producto);
        void Eliminar(int id);
    }
}
