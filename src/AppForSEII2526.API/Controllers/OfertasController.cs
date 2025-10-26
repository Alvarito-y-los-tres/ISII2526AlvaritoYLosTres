using AppForSEII2526.API.DTOs;
using AppForSEII2526.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AppForSEII2526.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OfertasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HerramientaController> _logger;
        public OfertasController(ApplicationDbContext context, ILogger<HerramientaController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        [Route("Oferta-Detalle")]
        [ProducesResponseType(typeof(IList<OfertaDetalleDTO>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetOfertaDetalle()
        {
            if (_context.Ofertas == null)
            {
                _logger.LogError("Error: La tabla no existe.");
                return NotFound();
            }

            IList<OfertaDetalleDTO> ofertaDetalles = await _context.Ofertas
                .Include(o => o.OfertaItems)
                .ThenInclude(oi => oi.Herramienta)
                .Select(o => new OfertaDetalleDTO(
                    o.FechaInicio,
                    o.FechaFin,
                    o.FechaOferta,
                    o.ParaSocio,
                    o.MetodoPago,
                    o.OfertaItems.Select(oi => new OfertaItemDTO(
                        oi.Herramienta.Nombre,
                        oi.Herramienta.Material,
                        oi.Herramienta.Fabricante.Nombre,
                        oi.Herramienta.Precio,
                        oi.PrecioFinal)).ToList()
                ))
                .ToListAsync();

            if (ofertaDetalles == null)
            {
                _logger.LogError("Error: No se encontraron ofertas.");
                return NotFound();
            }

            return Ok(ofertaDetalles);
        }
    }
}
