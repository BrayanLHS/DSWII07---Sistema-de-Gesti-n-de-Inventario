using System.ComponentModel.DataAnnotations;

namespace SistemaInventario.Models
{
    public class UsuarioViewModel
    {
        public int IdUsuario { get; set; }

        [Required(ErrorMessage = "Ingrese el nombre")]
        [RegularExpression(@"^[A-Za-zÁÉÍÓÚáéíóúÑñÜü]+$",
            ErrorMessage = "El nombre no debe tener espacios ni números")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ingrese el apellido")]
        [RegularExpression(@"^[A-Za-zÁÉÍÓÚáéíóúÑñÜü]+$",
            ErrorMessage = "El apellido no debe tener espacios ni números")]
        public string Apellido { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ingrese el correo")]
        [EmailAddress(ErrorMessage = "Ingrese un correo válido")]
        [RegularExpression(@"^[^@\s]+@[^@\s]+\.[a-zA-Z]{2,}$",
            ErrorMessage = "El formato del correo no es válido")]
        public string Correo { get; set; } = string.Empty;

        public string Clave { get; set; } = string.Empty;

        public string Rol { get; set; } = "Usuario";

        public bool Activo { get; set; } = true;

        public DateTime FechaRegistro { get; set; }
    }
}
