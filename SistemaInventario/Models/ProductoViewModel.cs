using System.ComponentModel.DataAnnotations;

namespace SistemaInventario.Models
{
    public class ProductoViewModel
    {
        public int IdProducto { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "Selecciona una categoría")]
        [Display(Name = "Categoría")]
        public int IdCategoria { get; set; }

        public string? NombreCategoria { get; set; }

        [Display(Name = "Proveedor")]
        public int? IdProveedor { get; set; }

        public string? NombreProveedor { get; set; }

        [Range(0.01, 1000000, ErrorMessage = "El precio debe ser mayor a 0")]
        public decimal Precio { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo")]
        public int Stock { get; set; }
    }
}
