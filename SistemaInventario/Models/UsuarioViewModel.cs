namespace SistemaInventario.Models
{
    public class UsuarioViewModel
    {
        public int IdUsuario { get; set; }

        public string Nombre { get; set; } = string.Empty;

        public string Correo { get; set; } = string.Empty;

        public string Clave { get; set; } = string.Empty;
    }
}