using SistemaInventario.Models;

namespace SistemaInventario.Data.Interfaces
{
    public interface IMovimientoRepositorio
    {
        List<MovimientoViewModel> Listar(int? idProducto = null);
        List<MovimientoViewModel> ListarPaginado(int? idProducto, int pagina, int tamano, out int total);
        void Registrar(MovimientoViewModel movimiento);

        // Registra solo el asiento de Kardex (Entrada) para el stock inicial de un producto recien creado.
        // No actualiza Producto.Stock porque ya se inserto con ese valor.
        void RegistrarStockInicial(int idProducto, int cantidad);
    }
}
