using Microsoft.AspNetCore.Mvc;
using miproyecto;
using TuProyecto.Controllers;


namespace TuProyecto.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductoController : ControllerBase
    {
        private readonly ProductoRepository repo;

        public ProductoController()
        {
            repo = new ProductoRepository();
        }

        // acá van los endpoints
        [HttpPost]
        public IActionResult CrearProducto([FromBody] Productos prod)
        {
            var nuevo = repo.CrearProducto(prod);
            return CreatedAtAction(nameof(ObtenerProductoPorId), new { id = nuevo.idProducto }, nuevo);
        }

        [HttpPut("{id}")]
        public IActionResult ModificarProducto(int id, [FromBody] Productos prod)
        {
            var productoModificado = repo.modificarProducto(id, prod);
            if (productoModificado == null)
                return NotFound();

            return Ok(productoModificado);
        }

        [HttpGet]
        public IActionResult ObtenerTodos()
        {
            var lista = repo.GetAll();
            return Ok(lista);
        }

        [HttpGet("{id}")]
        public IActionResult ObtenerProductoPorId(int id)
        {
            var producto = repo.DetallesProducto(id);
            if (producto == null)
                return NotFound();

            return Ok(producto);
        }

        [HttpDelete("{id}")]
        public IActionResult EliminarProducto(int id)
        {
            var eliminado = repo.EliminarProducto(id);
            if (eliminado == null)
                return NotFound();

            return Ok(eliminado);
        }

    }
}
