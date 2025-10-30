using AppForSEII2526.API.Data;
using AppForSEII2526.API.DTOs;
namespace AppForSEII2526.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AlquileresController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HerramientaController> _logger;
        public AlquileresController(ApplicationDbContext context, ILogger<HerramientaController> logger)
        {
            _context = context;
            _logger = logger;
        }
        [HttpGet]
        [Route("Alquiler-Detalle")]
        [ProducesResponseType(typeof(IList<AlquilerDetalleDTO>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAlquilerDetalle()
        {
            if (_context.Alquileres == null)
            {
                _logger.LogError("Error: La tabla no existe.");
                return NotFound();
            }

            IList<AlquilerDetalleDTO> alquilerDetalles = await _context.Alquileres
                .Include(o => o.ApplicationUser)
                .Include(o => o.AlquilerItems)
                    .ThenInclude(oi => oi.Herramienta)
                    .ThenInclude(h => h.Fabricante)
                .Select(o => new AlquilerDetalleDTO(
                    o.ApplicationUser.NombreCliente,
                    o.ApplicationUser.ApellidoCliente,
                    o.DireccionEnvio,
                    o.FechaAlquiler,
                    o.PrecioTotal,
                    o.FechaInicio,
                    o.FechaFin,

                    o.AlquilerItems.Select(oi => new AlquilerItemDTO(
                        oi.Herramienta.Nombre,
                        oi.Herramienta.Material,
                        oi.Herramienta.Precio,
                        oi.Cantidad
                        )).ToList()
                ))
                .ToListAsync();
            if (alquilerDetalles == null)
            {
                _logger.LogError("No se encontraron alquileres en la base de datos.");
                return NotFound();
            }

            return Ok(alquilerDetalles);
        }
    }
}
