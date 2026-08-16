using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaInventario.Data.Repositories;

namespace SistemaInventario.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsuarioController : Controller
    {
        private readonly UsuarioRepositorio _repo;

        public UsuarioController(UsuarioRepositorio repo)
        {
            _repo = repo;
        }

        public IActionResult Index()
        {
            ViewData["Title"] = "Usuarios";
            return View(_repo.Listar());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Eliminar(int id)
        {
            if (EsUsuarioActual(id))
            {
                TempData["Error"] = "No puedes eliminar tu propia cuenta.";
                return RedirectToAction(nameof(Index));
            }

            _repo.Eliminar(id);
            TempData["Exito"] = "Usuario eliminado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        private bool EsUsuarioActual(int id)
        {
            var idActual = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return idActual == id.ToString();
        }
    }
}
