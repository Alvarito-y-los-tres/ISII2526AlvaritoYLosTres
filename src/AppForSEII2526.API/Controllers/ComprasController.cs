using AppForSEII2526.API.DTOs;
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
        private readonly ILogger<HerramientaController> _logger;
        public ComprasController(ApplicationDbContext context, ILogger<HerramientaController> logger)
        {
            _context = context;
            _logger = logger;
        }


        [HttpGet]
        [Route("Compra-Detalle")]
        [ProducesResponseType(typeof(IList<CompraDetalleDTO>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetCompraDetalle()
        {
            if (_context.Compras == null)
            {
                _logger.LogError("Error: La tabla no existe.");
                return NotFound();
            }

            IList<CompraDetalleDTO> compraDetalles = await _context.Compras
                .Include(o => o.MetodoPago)
                .Include(o => o.ApplicationUser)
                .Include(o => o.CompraItems)
                    .ThenInclude(oi => oi.Herramienta)
                    .ThenInclude(h => h.Fabricante)
                

                .Select(o => new CompraDetalleDTO(
                    o.ApplicationUser.NombreCliente,
                    o.ApplicationUser.ApellidoCliente,
                    o.DireccionEnvio,
                    o.PrecioTotal,
                    o.FechaCompra,
                    o.CompraItems.Select(oi => new CompraItemDTO(
                        oi.Herramienta.Nombre,
                        oi.Herramienta.Material,
                        oi.Herramienta.Precio,
                        oi.Descripcion,
                        oi.Cantidad
                        )).ToList()
                ))
                .ToListAsync();


            if (compraDetalles == null)
            {
                _logger.LogError("No se encontraron compras en la base de datos.");
                return NotFound();
            }

            return Ok(compraDetalles);

        }
    }



}
