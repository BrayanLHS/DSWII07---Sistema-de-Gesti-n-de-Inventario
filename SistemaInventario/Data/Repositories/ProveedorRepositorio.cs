using Microsoft.Data.SqlClient;
using SistemaInventario.Data.Interfaces;
using SistemaInventario.Models;

namespace SistemaInventario.Data.Repositories
{
    public class ProveedorRepositorio : IProveedorRepositorio
    {
        private readonly ConexionBD conexionBD;

        public ProveedorRepositorio(ConexionBD conexionBD)
        {
            this.conexionBD = conexionBD;
        }

        public List<ProveedorViewModel> Listar(string? buscar = null)
        {
            var lista = new List<ProveedorViewModel>();

            using SqlConnection cn = conexionBD.ObtenerConexion();
            cn.Open();

            var sql = @"SELECT pr.IdProveedor, pr.Nombre, pr.Contacto, pr.Telefono, COUNT(p.IdProducto) AS CantidadProductos
                        FROM Proveedor pr
                        LEFT JOIN Producto p ON p.IdProveedor = pr.IdProveedor";
            if (!string.IsNullOrWhiteSpace(buscar))
                sql += " WHERE pr.Nombre LIKE @buscar";
            sql += " GROUP BY pr.IdProveedor, pr.Nombre, pr.Contacto, pr.Telefono ORDER BY pr.Nombre";

            using SqlCommand cmd = new SqlCommand(sql, cn);
            if (!string.IsNullOrWhiteSpace(buscar))
                cmd.Parameters.AddWithValue("@buscar", $"%{buscar}%");

            using SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                lista.Add(new ProveedorViewModel
                {
                    IdProveedor = Convert.ToInt32(dr["IdProveedor"]),
                    Nombre = dr["Nombre"].ToString() ?? string.Empty,
                    Contacto = dr["Contacto"] as string,
                    Telefono = dr["Telefono"] as string,
                    CantidadProductos = Convert.ToInt32(dr["CantidadProductos"])
                });
            }

            return lista;
        }

        public ProveedorViewModel? ObtenerPorId(int id)
        {
            using SqlConnection cn = conexionBD.ObtenerConexion();
            cn.Open();

            using SqlCommand cmd = new SqlCommand(
                "SELECT IdProveedor, Nombre, Contacto, Telefono FROM Proveedor WHERE IdProveedor = @id", cn);
            cmd.Parameters.AddWithValue("@id", id);

            using SqlDataReader dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                return new ProveedorViewModel
                {
                    IdProveedor = Convert.ToInt32(dr["IdProveedor"]),
                    Nombre = dr["Nombre"].ToString() ?? string.Empty,
                    Contacto = dr["Contacto"] as string,
                    Telefono = dr["Telefono"] as string
                };
            }

            return null;
        }

        public int Insertar(ProveedorViewModel proveedor)
        {
            using SqlConnection cn = conexionBD.ObtenerConexion();
            cn.Open();

            using SqlCommand cmd = new SqlCommand(
                @"INSERT INTO Proveedor(Nombre, Contacto, Telefono) VALUES(@Nombre, @Contacto, @Telefono);
                  SELECT CAST(SCOPE_IDENTITY() AS INT);", cn);
            cmd.Parameters.AddWithValue("@Nombre", proveedor.Nombre);
            cmd.Parameters.AddWithValue("@Contacto", (object?)proveedor.Contacto ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Telefono", (object?)proveedor.Telefono ?? DBNull.Value);
            return (int)cmd.ExecuteScalar();
        }

        public void Actualizar(ProveedorViewModel proveedor)
        {
            using SqlConnection cn = conexionBD.ObtenerConexion();
            cn.Open();

            using SqlCommand cmd = new SqlCommand(
                @"UPDATE Proveedor SET Nombre = @Nombre, Contacto = @Contacto, Telefono = @Telefono
                  WHERE IdProveedor = @IdProveedor", cn);
            cmd.Parameters.AddWithValue("@Nombre", proveedor.Nombre);
            cmd.Parameters.AddWithValue("@Contacto", (object?)proveedor.Contacto ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Telefono", (object?)proveedor.Telefono ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@IdProveedor", proveedor.IdProveedor);
            cmd.ExecuteNonQuery();
        }

        public void Eliminar(int id)
        {
            using SqlConnection cn = conexionBD.ObtenerConexion();
            cn.Open();

            using SqlCommand cmd = new SqlCommand(
                "DELETE FROM Proveedor WHERE IdProveedor = @id", cn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }
    }
}
