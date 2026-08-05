using Microsoft.Data.SqlClient;
using SistemaInventario.Data.Interfaces;
using SistemaInventario.Models;

namespace SistemaInventario.Data.Repositories
{
    public class CategoriaRepositorio : ICategoriaRepositorio
    {
        private readonly ConexionBD conexionBD;

        public CategoriaRepositorio(ConexionBD conexionBD)
        {
            this.conexionBD = conexionBD;
        }

        public List<CategoriaViewModel> Listar()
        {
            var lista = new List<CategoriaViewModel>();

            using SqlConnection cn = conexionBD.ObtenerConexion();
            cn.Open();

            using SqlCommand cmd = new SqlCommand(
                "SELECT IdCategoria, Nombre FROM Categoria ORDER BY Nombre", cn);

            using SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                lista.Add(new CategoriaViewModel
                {
                    IdCategoria = Convert.ToInt32(dr["IdCategoria"]),
                    Nombre = dr["Nombre"].ToString() ?? string.Empty
                });
            }

            return lista;
        }

        public CategoriaViewModel? ObtenerPorId(int id)
        {
            using SqlConnection cn = conexionBD.ObtenerConexion();
            cn.Open();

            using SqlCommand cmd = new SqlCommand(
                "SELECT IdCategoria, Nombre FROM Categoria WHERE IdCategoria = @id", cn);
            cmd.Parameters.AddWithValue("@id", id);

            using SqlDataReader dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                return new CategoriaViewModel
                {
                    IdCategoria = Convert.ToInt32(dr["IdCategoria"]),
                    Nombre = dr["Nombre"].ToString() ?? string.Empty
                };
            }

            return null;
        }

        public void Insertar(CategoriaViewModel categoria)
        {
            using SqlConnection cn = conexionBD.ObtenerConexion();
            cn.Open();

            using SqlCommand cmd = new SqlCommand(
                "INSERT INTO Categoria(Nombre) VALUES(@Nombre)", cn);
            cmd.Parameters.AddWithValue("@Nombre", categoria.Nombre);
            cmd.ExecuteNonQuery();
        }

        public void Actualizar(CategoriaViewModel categoria)
        {
            using SqlConnection cn = conexionBD.ObtenerConexion();
            cn.Open();

            using SqlCommand cmd = new SqlCommand(
                "UPDATE Categoria SET Nombre = @Nombre WHERE IdCategoria = @IdCategoria", cn);
            cmd.Parameters.AddWithValue("@Nombre", categoria.Nombre);
            cmd.Parameters.AddWithValue("@IdCategoria", categoria.IdCategoria);
            cmd.ExecuteNonQuery();
        }

        public void Eliminar(int id)
        {
            using SqlConnection cn = conexionBD.ObtenerConexion();
            cn.Open();

            using SqlCommand cmd = new SqlCommand(
                "DELETE FROM Categoria WHERE IdCategoria = @id", cn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }
    }
}
