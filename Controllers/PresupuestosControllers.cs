using Microsoft.AspNetCore.Mvc;
using TuProyecto.Controllers;

namespace TuProyecto.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PresupuestoController : ControllerBase
    {
        private readonly PresupuestosRepository repo;

        public PresupuestoController()
        {
            repo = new PresupuestosRepository();
        }

        [HttpPost]
        public IActionResult CrearPresupuesto([FromBody] Presupuestos presupuesto)
        {
            var nuevo = repo.Crearpresupuesto(presupuesto);
            return CreatedAtAction(nameof(ObtenerPresupuestoPorId), new { id = nuevo.idPresupuestos }, nuevo);
        }

        [HttpPost("{id}/ProductoDetalle")]
        public IActionResult AgregarProductoAlPresupuesto(int id, [FromBody] PresupuestosDetalles detalle)
        {
            var agregado = repo.AgregarProductoAlPresupuesto(id, detalle);

            if (agregado == null)
                return NotFound(new { mensaje = "Presupuesto o producto no encontrado." });

            return Ok(new
            {
                mensaje = "Producto agregado correctamente al presupuesto.",
                detalleAgregado = agregado
            });
        }


        [HttpGet("{id}")]
        public IActionResult ObtenerPresupuestoPorId(int id)
        {
            var presupuesto = repo.PresupuestoId(id);
            if (presupuesto == null)
                return NotFound();

            return Ok(presupuesto);
        }

        [HttpGet]
        public IActionResult ObtenerTodos()
        {
            var lista = repo.GetAll();
            return Ok(lista);
        }

        [HttpDelete("{id}")]
        public IActionResult EliminarPresupuesto(int id)
        {
            var eliminado = repo.EliminarPresupuestos(id);
            if (!eliminado)
                return NotFound(new { mensaje = "Presupuesto no encontrado" });

            return Ok(new { mensaje = "Presupuesto eliminado correctamente" });
        }

    }
}
