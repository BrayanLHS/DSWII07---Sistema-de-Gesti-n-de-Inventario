using Microsoft.Data.SqlClient;
using SistemaInventario.Data.Interfaces;
using SistemaInventario.Models;

namespace SistemaInventario.Data.Repositories
{
    public class ProductoRepositorio : IProductoRepositorio
    {
        private readonly ConexionBD conexionBD;

        public ProductoRepositorio(ConexionBD conexionBD)
        {
            this.conexionBD = conexionBD;
        }

        private const string SelectBase = @"
            SELECT p.IdProducto, p.Nombre, p.IdCategoria, c.Nombre AS NombreCategoria,
                   p.IdProveedor, pr.Nombre AS NombreProveedor, p.Precio, p.Stock
            FROM Producto p
            INNER JOIN Categoria c ON c.IdCategoria = p.IdCategoria
            LEFT JOIN Proveedor pr ON pr.IdProveedor = p.IdProveedor";

        private static ProductoViewModel Mapear(SqlDataReader dr) => new ProductoViewModel
        {
            IdProducto = Convert.ToInt32(dr["IdProducto"]),
            Nombre = dr["Nombre"].ToString() ?? string.Empty,
            IdCategoria = Convert.ToInt32(dr["IdCategoria"]),
            NombreCategoria = dr["NombreCategoria"] as string,
            IdProveedor = dr["IdProveedor"] is DBNull ? null : Convert.ToInt32(dr["IdProveedor"]),
            NombreProveedor = dr["NombreProveedor"] as string,
            Precio = Convert.ToDecimal(dr["Precio"]),
            Stock = Convert.ToInt32(dr["Stock"])
        };

        public List<ProductoViewModel> Listar(string? buscar = null, int? idCategoria = null, int? idProveedor = null, int? stockMenorQue = null)
        {
            List<ProductoViewModel> lista = new List<ProductoViewModel>();

            using SqlConnection cn = conexionBD.ObtenerConexion();
            cn.Open();

            var condiciones = new List<string>();
            if (!string.IsNullOrWhiteSpace(buscar))
                condiciones.Add("p.Nombre LIKE @buscar");
            if (idCategoria.HasValue)
                condiciones.Add("p.IdCategoria = @idCategoria");
            if (idProveedor.HasValue)
                condiciones.Add("p.IdProveedor = @idProveedor");
            if (stockMenorQue.HasValue)
                condiciones.Add("p.Stock < @stockMenorQue");

            var sql = SelectBase;
            if (condiciones.Count > 0)
                sql += " WHERE " + string.Join(" AND ", condiciones);
            sql += " ORDER BY p.IdProducto";

            using SqlCommand cmd = new SqlCommand(sql, cn);
            if (!string.IsNullOrWhiteSpace(buscar))
                cmd.Parameters.AddWithValue("@buscar", $"%{buscar}%");
            if (idCategoria.HasValue)
                cmd.Parameters.AddWithValue("@idCategoria", idCategoria.Value);
            if (idProveedor.HasValue)
                cmd.Parameters.AddWithValue("@idProveedor", idProveedor.Value);
            if (stockMenorQue.HasValue)
                cmd.Parameters.AddWithValue("@stockMenorQue", stockMenorQue.Value);

            using SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
                lista.Add(Mapear(dr));

            return lista;
        }

        public List<ProductoViewModel> ListarPaginado(int pagina, int tamano, out int total)
        {
            List<ProductoViewModel> lista = new List<ProductoViewModel>();

            using SqlConnection cn = conexionBD.ObtenerConexion();
            cn.Open();

            using (var cmdTotal = new SqlCommand("SELECT COUNT(*) FROM Producto", cn))
                total = (int)cmdTotal.ExecuteScalar();

            var sql = SelectBase + @"
                ORDER BY p.IdProducto
                OFFSET @salto ROWS FETCH NEXT @tamano ROWS ONLY";

            using SqlCommand cmd = new SqlCommand(sql, cn);
            cmd.Parameters.AddWithValue("@salto", (pagina - 1) * tamano);
            cmd.Parameters.AddWithValue("@tamano", tamano);

            using SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
                lista.Add(Mapear(dr));

            return lista;
        }

        public ProductoViewModel? ObtenerPorId(int id)
        {
            using SqlConnection cn = conexionBD.ObtenerConexion();
            cn.Open();

            var sql = SelectBase + " WHERE p.IdProducto = @id";
            using SqlCommand cmd = new SqlCommand(sql, cn);
            cmd.Parameters.AddWithValue("@id", id);

            using SqlDataReader dr = cmd.ExecuteReader();
            return dr.Read() ? Mapear(dr) : null;
        }

        public int Insertar(ProductoViewModel producto)
        {
            using SqlConnection cn = conexionBD.ObtenerConexion();
            cn.Open();

            using SqlCommand cmd = new SqlCommand(
                @"INSERT INTO Producto(Nombre, IdCategoria, IdProveedor, Precio, Stock)
                  VALUES(@Nombre, @IdCategoria, @IdProveedor, @Precio, @Stock);
                  SELECT CAST(SCOPE_IDENTITY() AS INT);", cn);

            cmd.Parameters.AddWithValue("@Nombre", producto.Nombre);
            cmd.Parameters.AddWithValue("@IdCategoria", producto.IdCategoria);
            cmd.Parameters.AddWithValue("@IdProveedor", (object?)producto.IdProveedor ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Precio", producto.Precio);
            cmd.Parameters.AddWithValue("@Stock", producto.Stock);

            return (int)cmd.ExecuteScalar();
        }

        public void Actualizar(ProductoViewModel producto)
        {
            using SqlConnection cn = conexionBD.ObtenerConexion();
            cn.Open();

            using SqlCommand cmd = new SqlCommand(
                @"UPDATE Producto SET Nombre = @Nombre, IdCategoria = @IdCategoria,
                  IdProveedor = @IdProveedor, Precio = @Precio, Stock = @Stock
                  WHERE IdProducto = @IdProducto", cn);

            cmd.Parameters.AddWithValue("@Nombre", producto.Nombre);
            cmd.Parameters.AddWithValue("@IdCategoria", producto.IdCategoria);
            cmd.Parameters.AddWithValue("@IdProveedor", (object?)producto.IdProveedor ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Precio", producto.Precio);
            cmd.Parameters.AddWithValue("@Stock", producto.Stock);
            cmd.Parameters.AddWithValue("@IdProducto", producto.IdProducto);

            cmd.ExecuteNonQuery();
        }

        public void Eliminar(int id)
        {
            using SqlConnection cn = conexionBD.ObtenerConexion();
            cn.Open();

            using SqlCommand cmd = new SqlCommand("DELETE FROM Producto WHERE IdProducto = @id", cn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }
    }
}
