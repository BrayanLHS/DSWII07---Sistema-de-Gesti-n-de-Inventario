using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SistemaInventario.Data.Repositories;
using SistemaInventario.Models;

namespace SistemaInventario.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsuarioController : Controller
    {
        private readonly UsuarioRepositorio _repo;
        private readonly PasswordHasher<UsuarioViewModel> _passwordHasher = new();

        public UsuarioController(UsuarioRepositorio repo)
        {
            _repo = repo;
        }

        public IActionResult Index(string? buscar)
        {
            ViewData["Title"] = "Usuarios";
            ViewBag.Buscar = buscar;
            return View(_repo.Listar(buscar));
        }

        public IActionResult Detalle(int id)
        {
            var usuario = _repo.ObtenerPorId(id);
            return usuario == null ? NotFound() : View(usuario);
        }

        [HttpGet]
        public IActionResult Editar(int id)
        {
            var usuario = _repo.ObtenerPorId(id);
            return usuario == null ? NotFound() : View(usuario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Editar(UsuarioViewModel modelo)
        {
            if (!ModelState.IsValid) return View(modelo);

            if (_repo.ExisteCorreo(modelo.Correo, excluirId: modelo.IdUsuario))
            {
                ModelState.AddModelError("Correo", "Este correo ya está en uso por otra cuenta.");
                return View(modelo);
            }

            _repo.Actualizar(modelo);
            TempData["Exito"] = "Usuario actualizado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CambiarEstado(int id, bool activo)
        {
            if (EsUsuarioActual(id) && !activo)
            {
                TempData["Error"] = "No puedes desactivar tu propia cuenta.";
                return RedirectToAction(nameof(Index));
            }

            if (!activo)
            {
                var usuario = _repo.ObtenerPorId(id);
                if (usuario?.Rol == "Admin")
                {
                    TempData["Error"] = "No se puede desactivar una cuenta de administrador.";
                    return RedirectToAction(nameof(Index));
                }
            }

            _repo.CambiarEstado(id, activo);
            TempData["Exito"] = activo ? "Usuario activado correctamente." : "Usuario desactivado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ResetearClave(int id)
        {
            if (EsUsuarioActual(id))
            {
                TempData["Error"] = "No puedes resetear la contraseña de tu propia cuenta mientras tienes la sesión activa. Pide a otro administrador que lo haga por ti.";
                return RedirectToAction(nameof(Index));
            }

            var usuario = _repo.ObtenerPorId(id);
            if (usuario == null) return NotFound();

            string claveTemporal = GenerarClaveTemporal();
            usuario.Clave = _passwordHasher.HashPassword(usuario, claveTemporal);
            _repo.ActualizarClave(id, usuario.Clave);

            TempData["ClaveTemporal"] = $"Nueva contraseña temporal para {usuario.Correo}: {claveTemporal}";
            return RedirectToAction(nameof(Index));
        }

        private bool EsUsuarioActual(int id)
        {
            var idActual = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return idActual == id.ToString();
        }

        private static string GenerarClaveTemporal()
        {
            const string caracteres = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnpqrstuvwxyz23456789!@#$%";
            var bytes = RandomNumberGenerator.GetBytes(12);
            var clave = new char[12];
            for (int i = 0; i < clave.Length; i++)
                clave[i] = caracteres[bytes[i] % caracteres.Length];
            return new string(clave);
        }
    }
}
