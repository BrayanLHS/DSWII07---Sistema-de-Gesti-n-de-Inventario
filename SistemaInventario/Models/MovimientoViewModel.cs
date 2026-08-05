using System.ComponentModel.DataAnnotations;

namespace SistemaInventario.Models
{
    public class MovimientoViewModel
    {
        public int IdMovimiento { get; set; }

        [Required(ErrorMessage = "Selecciona un producto")]
        [Display(Name = "Producto")]
        public int IdProducto { get; set; }

        public string? NombreProducto { get; set; }

        [Required]
        [Display(Name = "Tipo de movimiento")]
        public string Tipo { get; set; } = "Entrada"; 

        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
        public int Cantidad { get; set; }

        [Display(Name = "Fecha")]
        public DateTime Fecha { get; set; } = DateTime.Now;

        [Display(Name = "Motivo")]
        public string? Motivo { get; set; }
    }
}
