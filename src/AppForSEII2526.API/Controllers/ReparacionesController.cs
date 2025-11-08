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
                        ri.DescripcionProblema,
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
        //post
        [HttpPost]
        [Route("Crear-Reparacion")]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> CrearReparacion([FromBody] CrearReparacionDTO dto)
        {
            _logger.LogInformation("Iniciando creación de una nueva reparación...");

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Datos inválidos al crear reparación.");
                return BadRequest(ModelState);
            }

            try
            {
                if (dto.Items == null || !dto.Items.Any())
                {
                    _logger.LogWarning("La lista de herramientas está vacía.");
                    return BadRequest("Debe incluir al menos una herramienta para reparar.");
                }

                var nombresHerramientas = dto.Items.Select(i => i.NombreHerramienta).ToList();

                var herramientas = await _context.Herramientas
                    .Include(h => h.Fabricante)
                    .Where(h => nombresHerramientas.Contains(h.Nombre))
                    .ToListAsync();

                if (herramientas.Count != dto.Items.Count)
                {
                    _logger.LogWarning("Una o más herramientas indicadas no existen en la base de datos.");
                    return BadRequest("Una o más herramientas seleccionadas no existen.");
                }

                _logger.LogInformation("Herramientas validadas correctamente.");

                decimal precioTotal = 0;
                var itemsReparacion = new List<ReparacionItem>();

                foreach (var item in dto.Items)
                {
                    var herramienta = herramientas.FirstOrDefault(h => h.Nombre == item.NombreHerramienta);
                    if (herramienta == null)
                    {
                        _logger.LogWarning($"La herramienta '{item.NombreHerramienta}' no fue encontrada.");
                        continue;
                    }

                    decimal subtotal = herramienta.Precio * item.Cantidad;
                    precioTotal += subtotal;

                    itemsReparacion.Add(new ReparacionItem
                    {
                        Herramienta = herramienta,
                        DescripcionProblema = item.DescripcionProblema,
                        Cantidad = item.Cantidad
                    });

                    _logger.LogInformation($"Añadida herramienta '{herramienta.Nombre}' x{item.Cantidad}. Subtotal: {subtotal:C2}");
                }

                var reparacion = new Reparacion
                {
                    FechaEntrega = dto.FechaEntrega,
                    MetodoPago = (TiposMetodosPago)dto.MetodoPago,
                    PrecioTotal = precioTotal,
                    ApplicationUser = new ApplicationUser
                    {
                        NombreCliente = dto.NombreCliente,
                        ApellidoCliente = dto.ApellidoCliente,
                        NumTelefono = dto.Telefono
                    },
                    ItemsReparacion = itemsReparacion
                };

                _context.Reparaciones.Add(reparacion);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Reparación creada correctamente para {dto.NombreCliente} {dto.ApellidoCliente}. Total: {precioTotal:C2}");

                return Ok("Reparación creada correctamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear la reparación.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error interno al crear la reparación.");
            }
        }
        //post
    }
}
