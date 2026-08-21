using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaInventario.Data.Interfaces;
using SistemaInventario.Models;

namespace SistemaInventario.Controllers
{
    [Authorize]
    public class CategoriaController : Controller
    {
        private readonly ICategoriaRepositorio _repo;
        private readonly IProductoRepositorio _productos;

        public CategoriaController(ICategoriaRepositorio repo, IProductoRepositorio productos)
        {
            _repo = repo;
            _productos = productos;
        }
        public IActionResult Index(string? buscar, int? verProductos)
        {
            ViewData["Title"] = "Categorías";
            ViewBag.Buscar = buscar;
            ViewBag.VerProductos = verProductos;
            if (verProductos.HasValue)
            {
                ViewBag.CategoriaSeleccionada = _repo.ObtenerPorId(verProductos.Value);
                ViewBag.ProductosDeCategoria = _productos.Listar(idCategoria: verProductos.Value);
            }
            return View(_repo.Listar(buscar));
        }

        [HttpGet]
        public IActionResult Registrar() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Registrar(CategoriaViewModel modelo)
        {
            if (!ModelState.IsValid) return View(modelo);
            _repo.Insertar(modelo);
            TempData["Exito"] = $"Categoría '{modelo.Nombre}' registrada.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Editar(int id)
        {
            var categoria = _repo.ObtenerPorId(id);
            return categoria == null ? NotFound() : View(categoria);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Editar(CategoriaViewModel modelo)
        {
            if (!ModelState.IsValid) return View(modelo);
            _repo.Actualizar(modelo);
            TempData["Exito"] = $"Categoría '{modelo.Nombre}' actualizada.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Eliminar(int id)
        {
            if (_repo.TieneProductosAsociados(id))
            {
                TempData["Error"] = "No se puede eliminar esta categoría porque tiene productos asociados. Reasigna o elimina esos productos primero.";
                return RedirectToAction(nameof(Index));
            }

            _repo.Eliminar(id);
            TempData["Exito"] = "Categoría eliminada.";
            return RedirectToAction(nameof(Index));
        }
    }
}
