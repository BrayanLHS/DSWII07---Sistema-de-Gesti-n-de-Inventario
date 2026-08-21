using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using SistemaInventario.Data.Repositories;
using SistemaInventario.Models;

namespace SistemaInventario.Controllers
{
    public class CuentaController : Controller
    {
        private readonly UsuarioRepositorio usuarioRepositorio;
        private readonly PasswordHasher<UsuarioViewModel> passwordHasher;
        public CuentaController(UsuarioRepositorio usuarioRepositorio)
        {
            this.usuarioRepositorio = usuarioRepositorio;
            passwordHasher = new PasswordHasher<UsuarioViewModel>();
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [EnableRateLimiting("login")]
        public async Task<IActionResult> Login(LoginViewModel modelo)
        {
            if (!ModelState.IsValid)
            {
                return View(modelo);
            }
            UsuarioViewModel? usuario =
                usuarioRepositorio.BuscarPorCorreo(modelo.Correo);
            if (usuario == null)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Correo o contraseña incorrectos");
                return View(modelo);
            }
            PasswordVerificationResult resultado =
                passwordHasher.VerifyHashedPassword(
                    usuario,
                    usuario.Clave,
                    modelo.Clave);
            if (resultado == PasswordVerificationResult.Failed)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Correo o contraseña incorrectos");
                return View(modelo);
            }
            if (!usuario.Activo)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Tu cuenta está desactivada. Contacta a un administrador.");
                return View(modelo);
            }
            List<Claim> claims = new List<Claim>
            {
                new Claim(
                    ClaimTypes.NameIdentifier,
                    usuario.IdUsuario.ToString()),
                new Claim(ClaimTypes.Name, $"{usuario.Nombre} {usuario.Apellido}".Trim()),
                new Claim(ClaimTypes.Email, usuario.Correo),
                new Claim(ClaimTypes.Role, usuario.Rol)
            };
            ClaimsIdentity identidad = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identidad));
            TempData["Mensaje"] = "Iniciaste sesión correctamente";
            return RedirectToAction("Index", "Dashboard");
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(RegisterViewModel modelo)
        {
            if (!ModelState.IsValid)
            {
                return View(modelo);
            }
            if (usuarioRepositorio.ExisteCorreo(modelo.Correo))
            {
                ModelState.AddModelError(
                    "Correo",
                    "Este correo ya está registrado");
                return View(modelo);
            }
            UsuarioViewModel usuario = new UsuarioViewModel
            {
                Nombre = modelo.Nombre,
                Apellido = modelo.Apellido,
                Correo = modelo.Correo
            };
            usuario.Clave =
                passwordHasher.HashPassword(usuario, modelo.Clave);

            usuarioRepositorio.Registrar(usuario);

            TempData["Mensaje"] =
                "Cuenta creada correctamente";
            return RedirectToAction("Login");
        }
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}