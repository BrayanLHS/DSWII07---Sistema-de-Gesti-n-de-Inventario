namespace SistemaInventario.Models
{
    public class ProductoViewModel
    {
        public int IdProducto { get; set; }

        public string Nombre { get; set; }

        public string Categoria { get; set; }

        public decimal Precio { get; set; }

        public int Stock { get; set; }
    }
}