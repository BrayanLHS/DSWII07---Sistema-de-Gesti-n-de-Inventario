using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using SistemaInventario.Data.Interfaces;
using SistemaInventario.Models;

namespace SistemaInventario.Controllers
{
    public class ProductoController : Controller
    {
        private readonly IProductoRepositorio _repo;
        private readonly ICategoriaRepositorio _categorias;
        private readonly IProveedorRepositorio _proveedores;

        public ProductoController(
            IProductoRepositorio repo,
            ICategoriaRepositorio categorias,
            IProveedorRepositorio proveedores)
        {
            _repo = repo;
            _categorias = categorias;
            _proveedores = proveedores;
        }

        // GET: /Producto?buscar=teclado&pagina=1
        public IActionResult Index(string? buscar, int pagina = 1)
        {
            const int tamano = 8;
            List<ProductoViewModel> productos;
            int total;

            if (!string.IsNullOrWhiteSpace(buscar))
            {
                productos = _repo.Listar(buscar);
                total = productos.Count;
            }
            else
            {
                productos = _repo.ListarPaginado(pagina, tamano, out total);
            }

            ViewBag.Buscar = buscar;
            ViewBag.Pagina = pagina;
            ViewBag.TotalPaginas = (int)Math.Ceiling(total / (double)tamano);
            ViewData["Title"] = "Catálogo de productos";

            return View(productos);
        }

        // GET: /Producto/Detalle/5
        public IActionResult Detalle(int id)
        {
            var producto = _repo.ObtenerPorId(id);
            return producto == null ? NotFound() : View(producto);
        }

        // GET: /Producto/Registrar
        [HttpGet]
        public IActionResult Registrar()
        {
            CargarListas();
            return View();
        }

        // POST: /Producto/Registrar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Registrar(ProductoViewModel modelo)
        {
            if (!ModelState.IsValid)
            {
                CargarListas();
                return View(modelo);
            }

            _repo.Insertar(modelo);
            TempData["Exito"] = $"Producto '{modelo.Nombre}' registrado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Producto/Editar/5
        [HttpGet]
        public IActionResult Editar(int id)
        {
            var producto = _repo.ObtenerPorId(id);
            if (producto == null) return NotFound();

            CargarListas();
            return View(producto);
        }

        // POST: /Producto/Editar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Editar(ProductoViewModel modelo)
        {
            if (!ModelState.IsValid)
            {
                CargarListas();
                return View(modelo);
            }

            _repo.Actualizar(modelo);
            TempData["Exito"] = $"Producto '{modelo.Nombre}' actualizado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // POST: /Producto/Eliminar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Eliminar(int id)
        {
            _repo.Eliminar(id);
            TempData["Exito"] = "Producto eliminado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Producto/ExportarExcel
        public IActionResult ExportarExcel()
        {
            var productos = _repo.Listar();

            using var workbook = new XLWorkbook();
            var hoja = workbook.Worksheets.Add("Productos");

            hoja.Cell(1, 1).Value = "Id";
            hoja.Cell(1, 2).Value = "Nombre";
            hoja.Cell(1, 3).Value = "Categoría";
            hoja.Cell(1, 4).Value = "Proveedor";
            hoja.Cell(1, 5).Value = "Precio (S/)";
            hoja.Cell(1, 6).Value = "Stock";
            hoja.Row(1).Style.Font.Bold = true;

            int fila = 2;
            foreach (var p in productos)
            {
                hoja.Cell(fila, 1).Value = p.IdProducto;
                hoja.Cell(fila, 2).Value = p.Nombre;
                hoja.Cell(fila, 3).Value = p.NombreCategoria ?? "-";
                hoja.Cell(fila, 4).Value = p.NombreProveedor ?? "-";
                hoja.Cell(fila, 5).Value = p.Precio;
                hoja.Cell(fila, 6).Value = p.Stock;
                fila++;
            }

            hoja.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);

            return File(stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"Productos_{DateTime.Now:yyyyMMdd_HHmm}.xlsx");
        }

        // GET: /Producto/ExportarPdf
        public IActionResult ExportarPdf()
        {
            var productos = _repo.Listar();

            var documento = Document.Create(contenedor =>
            {
                contenedor.Page(pagina =>
                {
                    pagina.Size(PageSizes.A4);
                    pagina.Margin(30);

                    pagina.Header()
                        .Text("Catálogo de Productos - Sistema de Inventario")
                        .FontSize(16)
                        .Bold();

                    pagina.Content().Table(tabla =>
                    {
                        tabla.ColumnsDefinition(columnas =>
                        {
                            columnas.ConstantColumn(30);
                            columnas.RelativeColumn(3);
                            columnas.RelativeColumn(2);
                            columnas.RelativeColumn(2);
                            columnas.RelativeColumn(1);
                            columnas.RelativeColumn(1);
                        });

                        tabla.Header(encabezado =>
                        {
                            encabezado.Cell().Text("Id");
                            encabezado.Cell().Text("Nombre");
                            encabezado.Cell().Text("Categoría");
                            encabezado.Cell().Text("Proveedor");
                            encabezado.Cell().Text("Precio S/");
                            encabezado.Cell().Text("Stock");
                        });

                        foreach (var p in productos)
                        {
                            tabla.Cell().Text(p.IdProducto.ToString());
                            tabla.Cell().Text(p.Nombre);
                            tabla.Cell().Text(p.NombreCategoria ?? "-");
                            tabla.Cell().Text(p.NombreProveedor ?? "-");
                            tabla.Cell().Text(p.Precio.ToString("N2"));
                            tabla.Cell().Text(p.Stock.ToString());
                        }
                    });

                    pagina.Footer()
                        .AlignCenter()
                        .Text($"Generado el {DateTime.Now:dd/MM/yyyy HH:mm}");
                });
            });

            var pdfBytes = documento.GeneratePdf();
            return File(pdfBytes, "application/pdf", $"Productos_{DateTime.Now:yyyyMMdd_HHmm}.pdf");
        }

        private void CargarListas()
        {
            ViewBag.Categorias = new SelectList(_categorias.Listar(), "IdCategoria", "Nombre");
            ViewBag.Proveedores = new SelectList(_proveedores.Listar(), "IdProveedor", "Nombre");
        }
    }
}
