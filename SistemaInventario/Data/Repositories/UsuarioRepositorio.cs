using Microsoft.Data.SqlClient;
using SistemaInventario.Models;

namespace SistemaInventario.Data.Repositories
{
    public class UsuarioRepositorio
    {
        private readonly ConexionBD conexionBD;
        public UsuarioRepositorio(ConexionBD conexionBD)
        {
            this.conexionBD = conexionBD;
        }
        public bool ExisteCorreo(string correo, int? excluirId = null)
        {
            using SqlConnection cn = conexionBD.ObtenerConexion();
            cn.Open();
            var sql = "SELECT COUNT(*) FROM Usuario WHERE Correo = @Correo";
            if (excluirId.HasValue)
                sql += " AND IdUsuario <> @ExcluirId";
            SqlCommand cmd = new SqlCommand(sql, cn);
            cmd.Parameters.AddWithValue("@Correo", correo);
            if (excluirId.HasValue)
                cmd.Parameters.AddWithValue("@ExcluirId", excluirId.Value);
            int cantidad = Convert.ToInt32(cmd.ExecuteScalar());
            return cantidad > 0;
        }
        public void Registrar(UsuarioViewModel usuario)
        {
            using SqlConnection cn = conexionBD.ObtenerConexion();
            cn.Open();
            SqlCommand cmd = new SqlCommand(
                @"INSERT INTO Usuario(Nombre, Apellido, Correo, Clave, Rol)
                  VALUES(@Nombre, @Apellido, @Correo, @Clave, 'Usuario')", cn);
            cmd.Parameters.AddWithValue("@Nombre", usuario.Nombre);
            cmd.Parameters.AddWithValue("@Apellido", usuario.Apellido);
            cmd.Parameters.AddWithValue("@Correo", usuario.Correo);
            cmd.Parameters.AddWithValue("@Clave", usuario.Clave);
            cmd.ExecuteNonQuery();
        }
        public UsuarioViewModel? BuscarPorCorreo(string correo)
        {
            using SqlConnection cn = conexionBD.ObtenerConexion();
            cn.Open();
            SqlCommand cmd = new SqlCommand(
                @"SELECT IdUsuario, Nombre, Apellido, Correo, Clave, Rol, Activo, FechaRegistro
                  FROM Usuario
                  WHERE Correo = @Correo", cn);
            cmd.Parameters.AddWithValue("@Correo", correo);
            using SqlDataReader dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                return Mapear(dr, incluirClave: true);
            }
            return null;
        }
        public UsuarioViewModel? ObtenerPorId(int id)
        {
            using SqlConnection cn = conexionBD.ObtenerConexion();
            cn.Open();
            SqlCommand cmd = new SqlCommand(
                @"SELECT IdUsuario, Nombre, Apellido, Correo, Rol, Activo, FechaRegistro
                  FROM Usuario
                  WHERE IdUsuario = @Id", cn);
            cmd.Parameters.AddWithValue("@Id", id);
            using SqlDataReader dr = cmd.ExecuteReader();
            return dr.Read() ? Mapear(dr, incluirClave: false) : null;
        }
        public List<UsuarioViewModel> Listar(string? buscar = null)
        {
            var lista = new List<UsuarioViewModel>();
            using SqlConnection cn = conexionBD.ObtenerConexion();
            cn.Open();
            var sql = @"SELECT IdUsuario, Nombre, Apellido, Correo, Rol, Activo, FechaRegistro
                        FROM Usuario";
            if (!string.IsNullOrWhiteSpace(buscar))
                sql += " WHERE Nombre LIKE @buscar OR Apellido LIKE @buscar OR Correo LIKE @buscar";
            sql += " ORDER BY Nombre";
            SqlCommand cmd = new SqlCommand(sql, cn);
            if (!string.IsNullOrWhiteSpace(buscar))
                cmd.Parameters.AddWithValue("@buscar", $"%{buscar}%");
            using SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                lista.Add(Mapear(dr, incluirClave: false));
            }
            return lista;
        }
        public void Actualizar(UsuarioViewModel usuario)
        {
            using SqlConnection cn = conexionBD.ObtenerConexion();
            cn.Open();
            SqlCommand cmd = new SqlCommand(
                @"UPDATE Usuario SET Nombre = @Nombre, Apellido = @Apellido, Correo = @Correo
                  WHERE IdUsuario = @Id", cn);
            cmd.Parameters.AddWithValue("@Nombre", usuario.Nombre);
            cmd.Parameters.AddWithValue("@Apellido", usuario.Apellido);
            cmd.Parameters.AddWithValue("@Correo", usuario.Correo);
            cmd.Parameters.AddWithValue("@Id", usuario.IdUsuario);
            cmd.ExecuteNonQuery();
        }
        public void CambiarEstado(int idUsuario, bool activo)
        {
            using SqlConnection cn = conexionBD.ObtenerConexion();
            cn.Open();
            SqlCommand cmd = new SqlCommand(
                "UPDATE Usuario SET Activo = @Activo WHERE IdUsuario = @Id", cn);
            cmd.Parameters.AddWithValue("@Activo", activo);
            cmd.Parameters.AddWithValue("@Id", idUsuario);
            cmd.ExecuteNonQuery();
        }
        public void ActualizarClave(int idUsuario, string claveHasheada)
        {
            using SqlConnection cn = conexionBD.ObtenerConexion();
            cn.Open();
            SqlCommand cmd = new SqlCommand(
                "UPDATE Usuario SET Clave = @Clave WHERE IdUsuario = @Id", cn);
            cmd.Parameters.AddWithValue("@Clave", claveHasheada);
            cmd.Parameters.AddWithValue("@Id", idUsuario);
            cmd.ExecuteNonQuery();
        }

        private static UsuarioViewModel Mapear(SqlDataReader dr, bool incluirClave)
        {
            return new UsuarioViewModel
            {
                IdUsuario = Convert.ToInt32(dr["IdUsuario"]),
                Nombre = dr["Nombre"].ToString() ?? string.Empty,
                Apellido = dr["Apellido"].ToString() ?? string.Empty,
                Correo = dr["Correo"].ToString() ?? string.Empty,
                Clave = incluirClave ? (dr["Clave"].ToString() ?? string.Empty) : string.Empty,
                Rol = dr["Rol"].ToString() ?? "Usuario",
                Activo = Convert.ToBoolean(dr["Activo"]),
                FechaRegistro = Convert.ToDateTime(dr["FechaRegistro"])
            };
        }
    }
}
