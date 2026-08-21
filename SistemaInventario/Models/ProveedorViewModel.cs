using System.ComponentModel.DataAnnotations;

namespace SistemaInventario.Models
{
    public class ProveedorViewModel
    {
        public int IdProveedor { get; set; }

        [Required(ErrorMessage = "El nombre del proveedor es obligatorio")]
        [Display(Name = "Nombre / Razón social")]
        public string Nombre { get; set; } = string.Empty;

        [Display(Name = "Persona de contacto")]
        public string? Contacto { get; set; }

        [Display(Name = "Teléfono")]
        public string? Telefono { get; set; }

        public int CantidadProductos { get; set; }
    }
}
