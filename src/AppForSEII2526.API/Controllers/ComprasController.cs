using AppForSEII2526.API.DTOs;
using AppForSEII2526.API.Models;
using Humanizer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;

namespace AppForSEII2526.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ComprasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ComprasController> _logger;
        public ComprasController(ApplicationDbContext context, ILogger<ComprasController> logger)
        {
            _context = context;
            _logger = logger;

        }


        [HttpGet]
        [Route("Compra-Detalle")]
        [ProducesResponseType(typeof(CompraDetalleDTO), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetCompraDetalle(int id)
        {
            _logger.LogInformation("Solicitud GET Compra-Detalle recibida para compra ID {CompraId}", id);

            if (_context.Compras == null)
            {
                _logger.LogError("La tabla Compras es null.");
                return NotFound();
            }

            var compra = await _context.Compras
                .Where(c => c.Id == id)
                .Include(c => c.CompraItems)
                    .ThenInclude(ci => ci.Herramienta)
                        .ThenInclude(h => h.Fabricante)
                .Select(c => new CompraDetalleDTO(
                    c.ApplicationUser.NombreCliente,
                    c.ApplicationUser.ApellidoCliente,
                    c.DireccionEnvio,
                    c.PrecioTotal,
                    c.FechaCompra,
                    c.CompraItems.Select(ci => new CompraItemDTO(
                        ci.HerramientaId,
                        ci.Herramienta.Nombre,
                        ci.Herramienta.Material,
                        ci.Herramienta.Precio,
                        ci.Descripcion,
                        ci.Cantidad,
                        ci.CompraId)).ToList()))
                .FirstOrDefaultAsync();

            if (compra == null)
            {
                _logger.LogWarning("No se encontró la compra con ID {CompraId}", id);
                return NotFound();
            }

            _logger.LogInformation("Compra con ID {CompraId} recuperada correctamente", id);
            return Ok(compra);
        }

        //A implementar
        [HttpPost]
        public async Task<IActionResult> CrearCompra([FromBody] CrearCompraDTO crearCompraDTO)
        {

            return BadRequest();
        }
    }
}