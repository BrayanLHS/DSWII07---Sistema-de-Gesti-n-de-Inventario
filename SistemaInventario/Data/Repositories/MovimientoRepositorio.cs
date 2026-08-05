using Microsoft.Data.SqlClient;
using SistemaInventario.Data.Interfaces;
using SistemaInventario.Models;

namespace SistemaInventario.Data.Repositories
{
    public class MovimientoRepositorio : IMovimientoRepositorio
    {
        private readonly ConexionBD conexionBD;

        public MovimientoRepositorio(ConexionBD conexionBD)
        {
            this.conexionBD = conexionBD;
        }

        public List<MovimientoViewModel> Listar(int? idProducto = null)
        {
            var lista = new List<MovimientoViewModel>();

            using SqlConnection cn = conexionBD.ObtenerConexion();
            cn.Open();

            var sql = @"SELECT m.IdMovimiento, m.IdProducto, p.Nombre AS NombreProducto,
                               m.Tipo, m.Cantidad, m.Fecha, m.Motivo
                        FROM MovimientoInventario m
                        INNER JOIN Producto p ON p.IdProducto = m.IdProducto";

            if (idProducto.HasValue)
                sql += " WHERE m.IdProducto = @idProducto";
            sql += " ORDER BY m.Fecha DESC, m.IdMovimiento DESC";

            using SqlCommand cmd = new SqlCommand(sql, cn);
            if (idProducto.HasValue)
                cmd.Parameters.AddWithValue("@idProducto", idProducto.Value);

            using SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                lista.Add(new MovimientoViewModel
                {
                    IdMovimiento = Convert.ToInt32(dr["IdMovimiento"]),
                    IdProducto = Convert.ToInt32(dr["IdProducto"]),
                    NombreProducto = dr["NombreProducto"].ToString() ?? string.Empty,
                    Tipo = dr["Tipo"].ToString() ?? string.Empty,
                    Cantidad = Convert.ToInt32(dr["Cantidad"]),
                    Fecha = Convert.ToDateTime(dr["Fecha"]),
                    Motivo = dr["Motivo"] as string
                });
            }

            return lista;
        }

        public void Registrar(MovimientoViewModel movimiento)
        {
            using SqlConnection cn = conexionBD.ObtenerConexion();
            cn.Open();
            using SqlTransaction tran = cn.BeginTransaction();

            try
            {
                // Si es salida, validar que haya stock suficiente
                if (movimiento.Tipo == "Salida")
                {
                    using var cmdStock = new SqlCommand(
                        "SELECT Stock FROM Producto WHERE IdProducto = @id", cn, tran);
                    cmdStock.Parameters.AddWithValue("@id", movimiento.IdProducto);
                    var resultado = cmdStock.ExecuteScalar();

                    if (resultado == null)
                        throw new InvalidOperationException("El producto no existe.");

                    var stockActual = Convert.ToInt32(resultado);
                    if (stockActual < movimiento.Cantidad)
                        throw new InvalidOperationException(
                            $"Stock insuficiente. Stock actual: {stockActual}, se intenta retirar: {movimiento.Cantidad}.");
                }

                using (var cmdInsert = new SqlCommand(
                    @"INSERT INTO MovimientoInventario (IdProducto, Tipo, Cantidad, Fecha, Motivo)
                      VALUES (@IdProducto, @Tipo, @Cantidad, @Fecha, @Motivo)", cn, tran))
                {
                    cmdInsert.Parameters.AddWithValue("@IdProducto", movimiento.IdProducto);
                    cmdInsert.Parameters.AddWithValue("@Tipo", movimiento.Tipo);
                    cmdInsert.Parameters.AddWithValue("@Cantidad", movimiento.Cantidad);
                    cmdInsert.Parameters.AddWithValue("@Fecha", movimiento.Fecha);
                    cmdInsert.Parameters.AddWithValue("@Motivo", (object?)movimiento.Motivo ?? DBNull.Value);
                    cmdInsert.ExecuteNonQuery();
                }

                var signo = movimiento.Tipo == "Entrada" ? 1 : -1;
                using (var cmdUpdate = new SqlCommand(
                    "UPDATE Producto SET Stock = Stock + @delta WHERE IdProducto = @id", cn, tran))
                {
                    cmdUpdate.Parameters.AddWithValue("@delta", signo * movimiento.Cantidad);
                    cmdUpdate.Parameters.AddWithValue("@id", movimiento.IdProducto);
                    cmdUpdate.ExecuteNonQuery();
                }

                tran.Commit();
            }
            catch
            {
                tran.Rollback();
                throw;
            }
        }
    }
}
