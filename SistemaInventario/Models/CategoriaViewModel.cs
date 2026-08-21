using System.ComponentModel.DataAnnotations;

namespace SistemaInventario.Models
{
    public class CategoriaViewModel
    {
        public int IdCategoria { get; set; }

        [Required(ErrorMessage = "El nombre de la categoría es obligatorio")]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; } = string.Empty;

        public int CantidadProductos { get; set; }
    }
}
