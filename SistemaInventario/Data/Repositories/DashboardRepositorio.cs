using Microsoft.Data.SqlClient;
using SistemaInventario.Data.Interfaces;
using SistemaInventario.Models;

namespace SistemaInventario.Data.Repositories
{
    public class DashboardRepositorio : IDashboardRepositorio
    {
        private readonly ConexionBD conexionBD;

        public DashboardRepositorio(ConexionBD conexionBD)
        {
            this.conexionBD = conexionBD;
        }

        public DashboardViewModel ObtenerResumen(int stockMinimo = 10)
        {
            var resumen = new DashboardViewModel();

            using SqlConnection cn = conexionBD.ObtenerConexion();
            cn.Open();

            using (var cmd = new SqlCommand(
                "SELECT ISNULL(SUM(Precio * Stock), 0), COUNT(*) FROM Producto", cn))
            {
                using var dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    resumen.ValorTotalInventario = Convert.ToDecimal(dr[0]);
                    resumen.TotalProductos = Convert.ToInt32(dr[1]);
                }
            }

            using (var cmd = new SqlCommand(
                "SELECT COUNT(*) FROM Producto WHERE Stock < @minimo", cn))
            {
                cmd.Parameters.AddWithValue("@minimo", stockMinimo);
                resumen.CantidadStockBajo = (int)cmd.ExecuteScalar();
            }

            using (var cmd = new SqlCommand(
                @"SELECT c.Nombre, COUNT(p.IdProducto), ISNULL(SUM(p.Precio * p.Stock), 0)
                  FROM Categoria c
                  LEFT JOIN Producto p ON p.IdCategoria = c.IdCategoria
                  GROUP BY c.Nombre
                  ORDER BY ISNULL(SUM(p.Precio * p.Stock), 0) DESC", cn))
            {
                using var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    resumen.TopCategorias.Add(new CategoriaResumen
                    {
                        Nombre = dr.GetString(0),
                        CantidadProductos = dr.GetInt32(1),
                        ValorInventario = Convert.ToDecimal(dr[2])
                    });
                }
            }

            return resumen;
        }
    }
}
