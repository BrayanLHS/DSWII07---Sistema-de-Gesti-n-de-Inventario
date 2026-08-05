using Microsoft.AspNetCore.Mvc;
using SistemaInventario.Data.Interfaces;
using SistemaInventario.Models;

namespace SistemaInventario.Controllers
{
    // [ApiController] activa validación automática y binding propio de una API.
    // Ruta base: /api/productosapi
    [ApiController]
    [Route("api/[controller]")]
    public class ProductosApiController : ControllerBase   // ControllerBase = sin vistas
    {
        private readonly IProductoRepositorio _repo;

        public ProductosApiController(IProductoRepositorio repo)
        {
            _repo = repo;
        }

        // GET: /api/productosapi -> lista todos (JSON)
        [HttpGet]
        public ActionResult<IEnumerable<ProductoViewModel>> Get()
            => Ok(_repo.Listar());

        // GET: /api/productosapi/5 -> uno por id
        [HttpGet("{id}")]
        public ActionResult<ProductoViewModel> Get(int id)
        {
            var producto = _repo.ObtenerPorId(id);
            return producto is null ? NotFound() : Ok(producto);
        }

        // POST: /api/productosapi -> crea
        [HttpPost]
        public IActionResult Post([FromBody] ProductoViewModel modelo)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            _repo.Insertar(modelo);
            return CreatedAtAction(nameof(Get), new { id = modelo.IdProducto }, modelo);
        }

        // PUT: /api/productosapi/5 -> actualiza
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] ProductoViewModel modelo)
        {
            modelo.IdProducto = id;
            _repo.Actualizar(modelo);
            return NoContent();
        }

        // DELETE: /api/productosapi/5 -> elimina
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            _repo.Eliminar(id);
            return NoContent();
        }
    }
}
