using Microsoft.Data.SqlClient;
using SistemaInventario.Models;
using System.Data;

namespace SistemaInventario.Data
{
    public class ProductoRepositorio
    {
        private readonly ConexionBD conexionBD;

        public ProductoRepositorio(ConexionBD conexionBD)
        {
            this.conexionBD = conexionBD;
        }

        public List<ProductoViewModel> Listar()
        {
            List<ProductoViewModel> lista = new List<ProductoViewModel>();

            using (SqlConnection cn = conexionBD.ObtenerConexion())
            {
                cn.Open();

                SqlCommand cmd = new SqlCommand("SELECT * FROM Producto", cn);

                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    lista.Add(new ProductoViewModel
                    {
                        IdProducto = Convert.ToInt32(dr["IdProducto"]),
                        Nombre = dr["Nombre"].ToString(),
                        Categoria = dr["Categoria"].ToString(),
                        Precio = Convert.ToDecimal(dr["Precio"]),
                        Stock = Convert.ToInt32(dr["Stock"])
                    });
                }
            }

            return lista;
        }

        public void Registrar(ProductoViewModel producto)
        {
            using (SqlConnection cn = conexionBD.ObtenerConexion())
            {
                cn.Open();

                SqlCommand cmd = new SqlCommand(
                    "INSERT INTO Producto(Nombre,Categoria,Precio,Stock) VALUES(@Nombre,@Categoria,@Precio,@Stock)",
                    cn);

                cmd.Parameters.AddWithValue("@Nombre", producto.Nombre);
                cmd.Parameters.AddWithValue("@Categoria", producto.Categoria);
                cmd.Parameters.AddWithValue("@Precio", producto.Precio);
                cmd.Parameters.AddWithValue("@Stock", producto.Stock);

                cmd.ExecuteNonQuery();
            }
        }
    }
}