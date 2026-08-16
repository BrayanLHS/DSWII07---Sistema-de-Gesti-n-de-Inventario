using System.ComponentModel.DataAnnotations;

namespace SistemaInventario.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Ingrese su nombre")]
        [RegularExpression(@"^[A-Za-zÁÉÍÓÚáéíóúÑñÜü]+$",
            ErrorMessage = "El nombre no debe tener espacios ni números")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ingrese su apellido")]
        [RegularExpression(@"^[A-Za-zÁÉÍÓÚáéíóúÑñÜü]+$",
            ErrorMessage = "El apellido no debe tener espacios ni números")]
        public string Apellido { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ingrese su correo")]
        [EmailAddress(ErrorMessage = "Ingrese un correo válido")]
        [RegularExpression(@"^[^@\s]+@[^@\s]+\.[a-zA-Z]{2,}$",
            ErrorMessage = "El formato del correo no es válido")]
        public string Correo { get; set; } = string.Empty;
        [Required(ErrorMessage = "Ingrese una contraseña")]
        [MinLength(6, ErrorMessage = "Debe tener al menos 6 caracteres")]
        [DataType(DataType.Password)]
        public string Clave { get; set; } = string.Empty;
        [Required(ErrorMessage = "Confirme su contraseña")]
        [Compare("Clave", ErrorMessage = "Las contraseñas no coinciden")]
        [DataType(DataType.Password)]
        public string ConfirmarClave { get; set; } = string.Empty;
    }
}