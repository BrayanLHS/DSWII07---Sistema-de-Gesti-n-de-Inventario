using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Http;
using SistemaInventario.Models;

// Controlador que consume el Web API (ProductosApiController) por HTTP
// Se uso para demostrar el consumo de un API desde un cliente web, pero no es necesario para la funcionalidad principal del sistema.   
namespace SistemaInventario.Controllers
{
    public class ClienteController : Controller
    {
        private readonly IHttpClientFactory _factory;

        public ClienteController(IHttpClientFactory factory)
        {
            _factory = factory;
        }
        public async Task<IActionResult> Index()
        {
            var client = _factory.CreateClient("api");

            var productos = await client
                .GetFromJsonAsync<List<ProductoViewModel>>("api/productosapi");

            return View(productos ?? new List<ProductoViewModel>());
        }

        public async Task<IActionResult> Detalle(int id)
        {
            var client = _factory.CreateClient("api");
            var respuesta = await client.GetAsync($"api/productosapi/{id}");

            if (!respuesta.IsSuccessStatusCode)
                return NotFound();

            var producto = await respuesta.Content.ReadFromJsonAsync<ProductoViewModel>();
            return View(producto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(ProductoViewModel modelo)
        {
            var client = _factory.CreateClient("api");
            var respuesta = await client.PostAsJsonAsync("api/productosapi", modelo);

            TempData["Exito"] = respuesta.IsSuccessStatusCode
                ? "Producto enviado al API correctamente."
                : "El API rechazó el producto.";

            return RedirectToAction(nameof(Index));
        }
    }
}
