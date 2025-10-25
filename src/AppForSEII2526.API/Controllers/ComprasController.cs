using AppForSEII2526.API.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

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
            [Route("[action]")]
            [ProducesResponseType(typeof(IList<CompraDetalleDTO>), (int)HttpStatusCode.OK)]
            [ProducesResponseType((int)HttpStatusCode.NotFound)]
            public async Task<IActionResult> GetDetalleHerramientasParaComprar(int id)
            {
                if (_context.Compras == null)
                {
                    _logger.LogError("Error: No se encontraron compras en la base de datos");
                    return NotFound("No hay compras registradas");
                }

            var compras = await _context.Compras
             .Where(c => c.Id == id)
                .Include(c => c.CompraItems) 
                 .ThenInclude(ci => ci.Herramienta)
                    .ThenInclude(h => h.Fabricante) 

             .Select(c => new CompraDetalleDTO(
                 c.Id, 
                 c.ApplicationUser.NombreCliente,
                 c.ApplicationUser.ApellidoCliente,
                 c.DireccionEnvio,
                 c.PrecioTotal,
                 c.FechaCompra,
                 c.CompraItems

                    .Select(ci => new CompraItemDTO(
                        ci.Herramienta.Id,
                        ci.Herramienta.Nombre,
                        ci.Herramienta.Material,
                        ci.Precio,
                        ci.Descripcion,
                        ci.Cantidad)).ToList<CompraItemDTO>()))
         .FirstOrDefaultAsync();

            /*
            var comprasDetalle = await _context.Compras
                .Include(c => c.MetodoPago)
                .Include(c => c.ApplicationUser)
                .Include(c => c.CompraItems)
                    .ThenInclude(ci => ci.Herramienta)
                        .ThenInclude(h => h.Fabricante)
                .ToListAsync();


            var compraDetalleDTOs = comprasDetalle.Select(c => new CompraDetalleDTO(
                c.ApplicationUser.NombreCliente,
                c.ApplicationUser.ApellidoCliente,
                c.DireccionEnvio,
                c.PrecioTotal,
                c.FechaCompra,
                c.CompraItems.Select(ci => new CompraItemDTO(
                    ci.Herramienta.Nombre,
                    ci.Herramienta.Material,
                    ci.Herramienta.Precio,
                    ci.Descripcion,
                    ci.Cantidad
                )).ToList()
            )).ToList();

            */

            if (compras == null)
                {
                    _logger.LogError("Error: No se pudieron mapear las compras a DTOs");
                    return NotFound("No se encontraron detalles de compras");
                }

                return Ok(compras);


            }
        }
    }

    
