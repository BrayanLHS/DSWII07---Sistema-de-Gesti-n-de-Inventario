using Microsoft.AspNetCore.Mvc;
using SistemaInventario.Data.Interfaces;
using SistemaInventario.Models;

namespace SistemaInventario.Controllers
{
    public class ProveedorController : Controller
    {
        private readonly IProveedorRepositorio _repo;
        private readonly IProductoRepositorio _productos;

        public ProveedorController(IProveedorRepositorio repo, IProductoRepositorio productos)
        {
            _repo = repo;
            _productos = productos;
        }

        // GET: /Proveedor?verProductos=2 -> muestra el panel de productos de ese proveedor al costado
        public IActionResult Index(string? buscar, int? verProductos)
        {
            ViewData["Title"] = "Proveedores";
            ViewBag.Buscar = buscar;
            ViewBag.VerProductos = verProductos;

            if (verProductos.HasValue)
            {
                ViewBag.ProveedorSeleccionado = _repo.ObtenerPorId(verProductos.Value);
                ViewBag.ProductosDeProveedor = _productos.Listar(idProveedor: verProductos.Value);
            }

            return View(_repo.Listar(buscar));
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

        // POST: /Proveedor/CrearRapido (AJAX) -> usado por el modal "+" en el formulario de Producto
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CrearRapido(ProveedorViewModel modelo)
        {
            if (string.IsNullOrWhiteSpace(modelo.Nombre))
                return BadRequest(new { mensaje = "El nombre es obligatorio." });

            int id = _repo.Insertar(modelo);
            return Json(new { id, nombre = modelo.Nombre });
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
