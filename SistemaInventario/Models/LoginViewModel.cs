using System.ComponentModel.DataAnnotations;

namespace SistemaInventario.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Ingrese su correo")]
        [EmailAddress(ErrorMessage = "Ingrese un correo válido")]
        public string Correo { get; set; } = string.Empty;
        [Required(ErrorMessage = "Ingrese su contraseña")]
        [DataType(DataType.Password)]
        public string Clave { get; set; } = string.Empty;
    }
}