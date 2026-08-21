using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaInventario.Data.Interfaces;
using SistemaInventario.Models;

namespace SistemaInventario.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ProductosApiController : ControllerBase
    {
        private readonly IProductoRepositorio _repo;

        public ProductosApiController(IProductoRepositorio repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public ActionResult<IEnumerable<ProductoViewModel>> Get()
            => Ok(_repo.Listar());

        [HttpGet("{id}")]
        public ActionResult<ProductoViewModel> Get(int id)
        {
            var producto = _repo.ObtenerPorId(id);
            return producto is null ? NotFound() : Ok(producto);
        }

        [HttpPost]
        public IActionResult Post([FromBody] ProductoViewModel modelo)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            _repo.Insertar(modelo);
            return CreatedAtAction(nameof(Get), new { id = modelo.IdProducto }, modelo);
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] ProductoViewModel modelo)
        {
            modelo.IdProducto = id;
            _repo.Actualizar(modelo);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            _repo.Eliminar(id);
            return NoContent();
        }
    }
}
