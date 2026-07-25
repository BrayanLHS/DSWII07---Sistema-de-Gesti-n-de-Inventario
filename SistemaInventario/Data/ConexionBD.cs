using Microsoft.Data.SqlClient;

namespace SistemaInventario.Data
{
    public class ConexionBD
    {
        private readonly string cadenaConexion;

        public ConexionBD(IConfiguration configuration)
        {
            cadenaConexion = configuration.GetConnectionString("CadenaSQL");
        }

        public SqlConnection ObtenerConexion()
        {
            return new SqlConnection(cadenaConexion);
        }
    }
}