using Microsoft.AspNetCore.Mvc;
using SistemaInventario.Data.Interfaces;
using SistemaInventario.Models;

namespace SistemaInventario.Controllers
{
    public class ProveedorController : Controller
    {
        private readonly IProveedorRepositorio _repo;

        public ProveedorController(IProveedorRepositorio repo)
        {
            _repo = repo;
        }

        public IActionResult Index()
        {
            ViewData["Title"] = "Proveedores";
            return View(_repo.Listar());
        }

        [HttpGet]
        public IActionResult Registrar() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Registrar(ProveedorViewModel modelo)
        {
            if (!ModelState.IsValid) return View(modelo);

            _repo.Insertar(modelo);
            TempData["Exito"] = $"Proveedor '{modelo.Nombre}' registrado.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Editar(int id)
        {
            var proveedor = _repo.ObtenerPorId(id);
            return proveedor == null ? NotFound() : View(proveedor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Editar(ProveedorViewModel modelo)
        {
            if (!ModelState.IsValid) return View(modelo);

            _repo.Actualizar(modelo);
            TempData["Exito"] = $"Proveedor '{modelo.Nombre}' actualizado.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Eliminar(int id)
        {
            _repo.Eliminar(id);
            TempData["Exito"] = "Proveedor eliminado.";
            return RedirectToAction(nameof(Index));
        }
    }
}
