using SistemaInventario.Models;

namespace SistemaInventario.Data.Interfaces
{
    public interface IMovimientoRepositorio
    {
        List<MovimientoViewModel> Listar(int? idProducto = null);
        void Registrar(MovimientoViewModel movimiento);
    }
}
