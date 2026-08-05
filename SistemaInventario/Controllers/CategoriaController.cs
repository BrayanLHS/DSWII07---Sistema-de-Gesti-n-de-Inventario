using Microsoft.AspNetCore.Mvc;
using SistemaInventario.Data.Interfaces;
using SistemaInventario.Models;

namespace SistemaInventario.Controllers
{
    public class CategoriaController : Controller
    {
        private readonly ICategoriaRepositorio _repo;

        public CategoriaController(ICategoriaRepositorio repo)
        {
            _repo = repo;
        }

        public IActionResult Index()
        {
            ViewData["Title"] = "Categorías";
            return View(_repo.Listar());
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
            _repo.Eliminar(id);
            TempData["Exito"] = "Categoría eliminada.";
            return RedirectToAction(nameof(Index));
        }
    }
}
