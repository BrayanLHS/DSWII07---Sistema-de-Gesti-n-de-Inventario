using Microsoft.AspNetCore.Mvc;
using SistemaInventario.Data.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace SistemaInventario.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly IDashboardRepositorio _repo;

        public DashboardController(IDashboardRepositorio repo)
        {
            _repo = repo;
        }

        public IActionResult Index()
        {
            ViewData["Title"] = "Dashboard";
            var resumen = _repo.ObtenerResumen(stockMinimo: 10);
            return View(resumen);
        }
    }
}
