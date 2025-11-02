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
                        oi.Herramienta.Precio * (1 - (decimal)(oi.Porcentaje / 100)),
                        oi.Porcentaje,
                        oi.OfertaId
                        )).ToList()
                ))
                .ToListAsync();

            if (ofertaDetalles == null)
            {
                _logger.LogError("Error: No se encontraron ofertas.");
                return NotFound();
            }

            return Ok(ofertaDetalles);
        }


        [HttpPost]
        [Route("Crear-Oferta")]
        [ProducesResponseType(typeof(OfertaDetalleDTO), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Conflict)]
        public async Task<IActionResult> CrearOferta([FromBody] CrearOfertaDTO crearOfertaDTO)
        {
            if (_context.Ofertas == null || _context.Herramientas == null)
            {
                _logger.LogError("Error: La tabla no existe.");
                return StatusCode(500,"Error al configurar la base de datos.");
            }

            if(crearOfertaDTO.FechaInicio <= DateTime.Now)
            {
                ModelState.AddModelError("FechaInicio", "La fecha de inicio debe ser futura.");
            }

            if (crearOfertaDTO.FechaInicio >= crearOfertaDTO.FechaFin)
            {
                ModelState.AddModelError("FechaFin", "La fecha de fin debe ser posterior a la fecha de inicio.");
            }

            if(crearOfertaDTO.Items == null || !crearOfertaDTO.Items.Any())
            {
                ModelState.AddModelError("Items", "La oferta debe contener al menos una herramienta.");
            }

            var nombreHerramienta = crearOfertaDTO.Items.Select(h => h.NombreHerramienta).Distinct().ToList();

            var herramientas = await _context.Herramientas
                .Include(f => f.Fabricante)
                .Where(h => nombreHerramienta.Contains(h.Nombre))
                .ToListAsync();

            TiposMetodosPago metodoPago;
            if(crearOfertaDTO.MetodoPago == 0)
            {
                metodoPago = TiposMetodosPago.TarjetaCredito;
            }
            else if(crearOfertaDTO.MetodoPago == 1)
            {
                metodoPago = TiposMetodosPago.Paypal;
            }
            else if(crearOfertaDTO.MetodoPago == 2)
            {
                metodoPago = TiposMetodosPago.Cash;
            }
            else
            {
                ModelState.AddModelError("MetodoPago", "El método de pago especificado no es válido. ¡Utilice 0, 1 o 2!");
                return ValidationProblem(ModelState);
            }

            TiposDirigidaOferta tiposDirigidaOferta;
            if(crearOfertaDTO.ParaSocio == 0)
            {
                tiposDirigidaOferta = TiposDirigidaOferta.Socios;
            }
            else if(crearOfertaDTO.ParaSocio == 1)
            {
                tiposDirigidaOferta = TiposDirigidaOferta.Clientes;
            }
            else
            {
                ModelState.AddModelError("ParaSocio", "El tipo de oferta especificado no es válido. ¡Utilice el 0 o 1!");
                return ValidationProblem(ModelState);
            }

            var usuario = await _context.Users.FirstOrDefaultAsync(u => u.NombreCliente == crearOfertaDTO.nombreUsuario);
            if (usuario == null)
            {
                _logger.LogWarning($"Usuario '{crearOfertaDTO.nombreUsuario}' no encontrado.");
                return NotFound($"El usuario '{crearOfertaDTO.nombreUsuario}' no existe.");
            }

            if (ModelState.ErrorCount > 0)
            {
                return BadRequest(new ValidationProblemDetails(ModelState));
            }
            

            var nuevaOferta = new Oferta
            {
                FechaInicio = crearOfertaDTO.FechaInicio,
                FechaFin = crearOfertaDTO.FechaFin,
                FechaOferta = crearOfertaDTO.FechaOferta,
                MetodoPago = metodoPago,
                ParaSocio = tiposDirigidaOferta,
                ApplicationUser = usuario,
                OfertaItems = new List<OfertaItem>()
            };

            foreach(var itemDTO in crearOfertaDTO.Items)
            {
                var herramienta = herramientas.FirstOrDefault(h => h.Nombre == itemDTO.NombreHerramienta);

                if (herramienta == null)
                {
                    ModelState.AddModelError("Items", $"La herramienta '{itemDTO.NombreHerramienta}' no existe.");
                    return ValidationProblem(ModelState);
                }

                var ofertaItem = new OfertaItem
                {
                    HerramientaId = herramienta.Id,
                    Herramienta = herramienta,
                    Porcentaje = itemDTO.Porcentaje,
                    PrecioFinal = herramienta.Precio * (1 - (decimal)(itemDTO.Porcentaje / 100))
                };
                nuevaOferta.OfertaItems.Add(ofertaItem);
            }

            _context.Ofertas.Add(nuevaOferta);
            await _context.SaveChangesAsync();


            var ofertaDetalleDTO = new OfertaDetalleDTO(
                nuevaOferta.FechaInicio,
                nuevaOferta.FechaFin,
                nuevaOferta.FechaOferta,
                nuevaOferta.ParaSocio,
                nuevaOferta.MetodoPago,
                nuevaOferta.OfertaItems.Select(oi => new OfertaItemDTO(
                    oi.Herramienta.Nombre,
                    oi.Herramienta.Material,
                    oi.Herramienta.Fabricante.Nombre,
                    oi.Herramienta.Precio,
                    oi.PrecioFinal,
                    oi.Porcentaje,
                    oi.OfertaId
                    )).ToList()
            );
            return Ok(ofertaDetalleDTO);


        }
    }
}
