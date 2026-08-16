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
        public bool ExisteCorreo(string correo)
        {
            using SqlConnection cn = conexionBD.ObtenerConexion();
            cn.Open();
            SqlCommand cmd = new SqlCommand(
                "SELECT COUNT(*) FROM Usuario WHERE Correo = @Correo", cn);
            cmd.Parameters.AddWithValue("@Correo", correo);
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
                @"SELECT IdUsuario, Nombre, Apellido, Correo, Clave, Rol
                  FROM Usuario
                  WHERE Correo = @Correo", cn);
            cmd.Parameters.AddWithValue("@Correo", correo);
            using SqlDataReader dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                return new UsuarioViewModel
                {
                    IdUsuario = Convert.ToInt32(dr["IdUsuario"]),
                    Nombre = dr["Nombre"].ToString() ?? string.Empty,
                    Apellido = dr["Apellido"].ToString() ?? string.Empty,
                    Correo = dr["Correo"].ToString() ?? string.Empty,
                    Clave = dr["Clave"].ToString() ?? string.Empty,
                    Rol = dr["Rol"].ToString() ?? "Usuario"
                };
            }
            return null;
        }
        public List<UsuarioViewModel> Listar()
        {
            var lista = new List<UsuarioViewModel>();
            using SqlConnection cn = conexionBD.ObtenerConexion();
            cn.Open();
            SqlCommand cmd = new SqlCommand(
                @"SELECT IdUsuario, Nombre, Apellido, Correo, Rol
                  FROM Usuario
                  ORDER BY Nombre", cn);
            using SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                lista.Add(new UsuarioViewModel
                {
                    IdUsuario = Convert.ToInt32(dr["IdUsuario"]),
                    Nombre = dr["Nombre"].ToString() ?? string.Empty,
                    Apellido = dr["Apellido"].ToString() ?? string.Empty,
                    Correo = dr["Correo"].ToString() ?? string.Empty,
                    Rol = dr["Rol"].ToString() ?? "Usuario"
                });
            }
            return lista;
        }
        public void Eliminar(int idUsuario)
        {
            using SqlConnection cn = conexionBD.ObtenerConexion();
            cn.Open();
            SqlCommand cmd = new SqlCommand(
                "DELETE FROM Usuario WHERE IdUsuario = @Id", cn);
            cmd.Parameters.AddWithValue("@Id", idUsuario);
            cmd.ExecuteNonQuery();
        }
    }
}