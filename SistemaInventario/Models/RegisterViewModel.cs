using System.ComponentModel.DataAnnotations;

namespace SistemaInventario.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Ingrese su nombre")]
        public string Nombre { get; set; } = string.Empty;
        [Required(ErrorMessage = "Ingrese su correo")]
        [EmailAddress(ErrorMessage = "Ingrese un correo válido")]
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