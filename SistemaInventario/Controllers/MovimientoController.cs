using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SistemaInventario.Data.Interfaces;
using SistemaInventario.Models;

namespace SistemaInventario.Controllers
{
    [Authorize]
    public class MovimientoController : Controller
    {
        private readonly IMovimientoRepositorio _repo;
        private readonly IProductoRepositorio _productos;

        public MovimientoController(IMovimientoRepositorio repo, IProductoRepositorio productos)
        {
            _repo = repo;
            _productos = productos;
        }

        public IActionResult Index(int? idProducto, int pagina = 1)
        {
            const int tamano = 10;

            ViewData["Title"] = "Kardex de movimientos";
            ViewBag.IdProducto = idProducto;
            if (idProducto.HasValue)
                ViewBag.Producto = _productos.ObtenerPorId(idProducto.Value);

            var movimientos = _repo.ListarPaginado(idProducto, pagina, tamano, out int total);
            ViewBag.Pagina = pagina;
            ViewBag.TotalPaginas = (int)Math.Ceiling(total / (double)tamano);

            return View(movimientos);
        }

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
