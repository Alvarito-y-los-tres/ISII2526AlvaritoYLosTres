using AppForSEII2526.API.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AppForSEII2526.API.Models;
using Microsoft.Extensions.Logging.Abstractions;


namespace AppForSEII2526.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReparacionesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HerramientaController> _logger;
        public ReparacionesController(ApplicationDbContext context, ILogger<HerramientaController> logger)
        {
            _context = context;
            _logger = logger;
        }

            [HttpGet]
        [Route("Reparacion-Detalle")]
        [ProducesResponseType(typeof(IList<ReparacionDetalleDTO>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetReparacionDetalle()
        {
            if (_context.Reparaciones == null)
            {
                _logger.LogError("Error: La tabla no existe.");
                return NotFound();
            }

            IList<ReparacionDetalleDTO> reparacionDetalles = await _context.Reparaciones

                .Include(r => r.ApplicationUser)
                .Include(r => r.ItemsReparacion)
                    .ThenInclude(ri => ri.Herramienta)
                    .ThenInclude(h => h.Fabricante)

                .Select(r => new ReparacionDetalleDTO(
                    r.ApplicationUser.NombreCliente,
                    r.ApplicationUser.ApellidoCliente,
                    r.PrecioTotal,
                    r.FechaEntrega,
                    r.FechaRecogida,
                    r.ItemsReparacion.Select(ri => new ReparacionItemDTO(
                        ri.Herramienta.Nombre,
                        ri.Herramienta.Precio,
                        ri.Descripcion,
                        ri.Cantidad
                    )).ToList()
                ))
                .ToListAsync();

            if (reparacionDetalles == null || !reparacionDetalles.Any())
            {
                _logger.LogError("Error: No se encontraron reparaciones.");
                return NotFound();
            }

            return Ok(reparacionDetalles);
        }

    }
}
