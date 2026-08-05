using SistemaInventario.Models;

namespace SistemaInventario.Data.Interfaces
{
    public interface IDashboardRepositorio
    {
        DashboardViewModel ObtenerResumen(int stockMinimo = 10);
    }
}
