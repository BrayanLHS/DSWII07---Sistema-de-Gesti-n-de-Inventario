using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Http;
using SistemaInventario.Models;

namespace SistemaInventario.Controllers
{
    // Este controlador NO toca la base de datos directamente:
    // consume el Web API (ProductosApiController) por HTTP,
    // tal como lo haría una app cliente externa.
    public class ClienteController : Controller
    {
        private readonly IHttpClientFactory _factory;

        public ClienteController(IHttpClientFactory factory)
        {
            _factory = factory;
        }

        // GET: /Cliente -> lista los productos consumiendo el API
        public async Task<IActionResult> Index()
        {
            var client = _factory.CreateClient("api");

            var productos = await client
                .GetFromJsonAsync<List<ProductoViewModel>>("api/productosapi");

            return View(productos ?? new List<ProductoViewModel>());
        }

        // GET: /Cliente/Detalle/5 -> consume un producto por id
        public async Task<IActionResult> Detalle(int id)
        {
            var client = _factory.CreateClient("api");
            var respuesta = await client.GetAsync($"api/productosapi/{id}");

            if (!respuesta.IsSuccessStatusCode)
                return NotFound();

            var producto = await respuesta.Content.ReadFromJsonAsync<ProductoViewModel>();
            return View(producto);
        }

        // POST: /Cliente/Crear -> envía un producto nuevo al API
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
