namespace SistemaInventario.Models
{
    public class CategoriaResumen
    {
        public string Nombre { get; set; } = string.Empty;
        public int CantidadProductos { get; set; }
        public decimal ValorInventario { get; set; }
    }

    public class DashboardViewModel
    {
        public decimal ValorTotalInventario { get; set; }
        public int TotalProductos { get; set; }
        public int CantidadStockBajo { get; set; }
        public List<CategoriaResumen> TopCategorias { get; set; } = new();
    }
}
