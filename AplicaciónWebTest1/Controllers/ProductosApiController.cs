using System.Threading.Tasks;
using AplicaciónWebTest1.Data;
using AplicaciónWebTest1.Models;
using Microsoft.AspNetCore.Mvc;

namespace AplicaciónWebTest1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductosApiController : ControllerBase
    {
        private readonly ProductoDAO _dao;

        public ProductosApiController(ProductoDAO dao)
        {
            _dao = dao;
        }

        // GET: api/productos
        [HttpGet]
        public async Task<IActionResult> GetAll(string? busqueda, string? ordenarPor = "ID", bool desc = false, int page = 1, int pageSize = 0)
        {
            var resultado = await _dao.ListarAsync(busqueda, ordenarPor, desc, page, pageSize);
            return Ok(resultado);
        }

        // GET: api/productos/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            if (id <= 0) return BadRequest("Id inválido");

            var producto = await _dao.ObtenerPorIdAsync(id);
            if (producto == null) return NotFound();

            return Ok(producto);
        }

        // POST: api/productos
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Producto producto)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            await _dao.InsertarAsync(producto);
            return Ok(producto);
        }

        // PUT: api/productos/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] Producto producto)
        {
            if (id != producto.Id) return BadRequest("Id inconsistente");
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            await _dao.ActualizarAsync(producto);
            return NoContent();
        }

        // DELETE: api/productos/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) return BadRequest("Id inválido");

            await _dao.BorrarAsync(id);
            return NoContent();
        }
    }
}
