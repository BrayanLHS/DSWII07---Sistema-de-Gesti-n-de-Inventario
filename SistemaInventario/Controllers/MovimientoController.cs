using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SistemaInventario.Data.Interfaces;
using SistemaInventario.Models;

namespace SistemaInventario.Controllers
{
    // Kardex: entradas y salidas de inventario, ligadas a un producto.
    public class MovimientoController : Controller
    {
        private readonly IMovimientoRepositorio _repo;
        private readonly IProductoRepositorio _productos;

        public MovimientoController(IMovimientoRepositorio repo, IProductoRepositorio productos)
        {
            _repo = repo;
            _productos = productos;
        }

        // GET: /Movimiento?idProducto=5
        public IActionResult Index(int? idProducto)
        {
            ViewData["Title"] = "Kardex de movimientos";
            ViewBag.IdProducto = idProducto;

            if (idProducto.HasValue)
                ViewBag.Producto = _productos.ObtenerPorId(idProducto.Value);

            return View(_repo.Listar(idProducto));
        }

        // Registrar movimiento: esta pantalla solo maneja Salidas.
        // Las Entradas se generan automaticamente al crear un producto con stock inicial (ProductoController.Registrar).
        [HttpGet]
        public IActionResult Registrar(int? idProducto)
        {
            CargarProductos();
            return View(new MovimientoViewModel
            {
                Fecha = DateTime.Now,
                IdProducto = idProducto ?? 0,
                Tipo = "Salida"
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Registrar(MovimientoViewModel modelo)
        {
            modelo.Tipo = "Salida";

            if (!ModelState.IsValid)
            {
                CargarProductos();
                return View(modelo);
            }

            try
            {
                _repo.Registrar(modelo);
                TempData["Exito"] = "Movimiento registrado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                CargarProductos();
                return View(modelo);
            }
        }

        private void CargarProductos()
        {
            var productos = _productos.Listar();
            ViewBag.Productos = new SelectList(productos, "IdProducto", "Nombre");
            ViewBag.StockPorProducto = productos.ToDictionary(p => p.IdProducto, p => p.Stock);
        }
    }
}
