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
                @"INSERT INTO Usuario(Nombre, Correo, Clave)
                  VALUES(@Nombre, @Correo, @Clave)", cn);
            cmd.Parameters.AddWithValue("@Nombre", usuario.Nombre);
            cmd.Parameters.AddWithValue("@Correo", usuario.Correo);
            cmd.Parameters.AddWithValue("@Clave", usuario.Clave);
            cmd.ExecuteNonQuery();
        }
        public UsuarioViewModel? BuscarPorCorreo(string correo)
        {
            using SqlConnection cn = conexionBD.ObtenerConexion();
            cn.Open();
            SqlCommand cmd = new SqlCommand(
                @"SELECT IdUsuario, Nombre, Correo, Clave
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
                    Correo = dr["Correo"].ToString() ?? string.Empty,
                    Clave = dr["Clave"].ToString() ?? string.Empty
                };
            }
            return null;
        }
    }
}