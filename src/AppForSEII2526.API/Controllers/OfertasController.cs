using AppForSEII2526.API.DTOs;
using AppForSEII2526.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;

namespace AppForSEII2526.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OfertasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<OfertasController> _logger;
        public OfertasController(ApplicationDbContext context, ILogger<OfertasController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        [Route("Oferta-Detalle")]
        [ProducesResponseType(typeof(IList<OfertaDetalleDTO>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetOfertaDetalle(int id)
        {
            if (_context.Ofertas == null)
            {
                _logger.LogError("Error: La tabla no existe.");
                return NotFound();
            }

            var ofertaDetalles = await _context.Ofertas
                .Where(o => o.Id == id)
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
                .FirstOrDefaultAsync();

            if (ofertaDetalles == null)
            {
                _logger.LogError($"No se encontró ninguna oferta con ID {id}.");
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

            if (crearOfertaDTO.FechaFin < crearOfertaDTO.FechaInicio.AddDays(7)){
                ModelState.AddModelError("FechaFin", "¡Error!, la oferta debe durar al menos una semana.");
            }


            var nombreHerramienta = crearOfertaDTO.Items.Select(h => h.NombreHerramienta).Distinct().ToList();

            var herramientas = await _context.Herramientas
                .Include(f => f.Fabricante)
                .Where(h => nombreHerramienta.Contains(h.Nombre))
                .ToListAsync();

            TiposMetodosPago metodoPago = TiposMetodosPago.TarjetaCredito;
            if (crearOfertaDTO.MetodoPago == 0)
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
            }

            TiposDirigidaOferta tiposDirigidaOferta = TiposDirigidaOferta.Clientes;
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
            }

            var usuario = await _context.Users.FirstOrDefaultAsync(u => u.NombreCliente == crearOfertaDTO.NombreUsuario);
            if (usuario == null)
            {
                ModelState.AddModelError("Usuario", "El usuario no existe.");
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
                    ModelState.AddModelError("Items", "La herramienta no existe.");
                    continue;
                }

                if (itemDTO.Porcentaje < 0 || itemDTO.Porcentaje > 100)
                    ModelState.AddModelError("Porcentaje", "El porcentaje debe estar entre 0 y 100.");

                var ofertaItem = new OfertaItem
                {
                    HerramientaId = herramienta.Id,
                    Herramienta = herramienta,
                    Porcentaje = itemDTO.Porcentaje,
                    PrecioFinal = herramienta.Precio * (1 - (decimal)(itemDTO.Porcentaje / 100))
                };
                nuevaOferta.OfertaItems.Add(ofertaItem);
            }

            if (ModelState.ErrorCount > 0)
            {
                return BadRequest(new ValidationProblemDetails(ModelState));
            }

            _context.Ofertas.Add(nuevaOferta);
            
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar la nueva oferta en la base de datos.");
                return Conflict("Error" + ex.Message);
            }


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
            return CreatedAtAction(nameof(GetOfertaDetalle), new { id = nuevaOferta.Id }, ofertaDetalleDTO);


        }
    }
}
