using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AppForSEII2526.API.Data;
using AppForSEII2526.API.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AppForSEII2526.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AlquileresController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AlquileresController> _logger;
        public AlquileresController(ApplicationDbContext context, ILogger<AlquileresController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        [Route("Alquiler-Detalle")]
        // FIX 1: El tipo de respuesta es UN solo DTO, no una lista.
        [ProducesResponseType(typeof(AlquilerDetalleDTO), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)] // Añadido para documentar el 404
        public async Task<IActionResult> GetAlquilerDetalle(int id)
        {
            if (_context.Alquileres == null)
            {
                _logger.LogError("Error: La tabla no existe.");
                return NotFound();
            }

            
            var alquiler = await _context.Alquileres
                .Where(o => o.Id == id) // <-- Filtra por ID aquí
                .Include(o => o.ApplicationUser)
                .Include(o => o.AlquilerItems)
                    .ThenInclude(oi => oi.Herramienta)
                    .ThenInclude(h => h.Fabricante)
                .FirstOrDefaultAsync(); 

            // FIX 3: Esta es la comprobación de 'null' que SÍ funciona.
            if (alquiler == null)
            {
                _logger.LogWarning("No se encontró alquiler con ID: {Id}", id);
                return NotFound(); // <-- Esto hará que tu test pase
            }

            // FIX 4: Mapear a DTO *después* de confirmar que existe.
            var alquilerDetalleDTO = new AlquilerDetalleDTO(
                alquiler.ApplicationUser.NombreCliente,
                alquiler.ApplicationUser.ApellidoCliente,
                alquiler.DireccionEnvio,
                alquiler.FechaAlquiler,
                alquiler.PrecioTotal,
                alquiler.FechaInicio,
                alquiler.FechaFin,
                alquiler.Id,
                alquiler.AlquilerItems.Select(oi => new AlquilerItemDTO(
                    oi.Herramienta.Nombre,
                    oi.Herramienta.Material,
                    oi.Herramienta.Precio,
                    oi.Cantidad
                    )).ToList()
            );

            return Ok(alquilerDetalleDTO);
        }

        [HttpPost]
        [Route("Crear-Alquiler")]
        [ProducesResponseType(typeof(AlquilerDetalleDTO), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Conflict)]
        public async Task<IActionResult> CrearAlquiler([FromBody] CrearAlquilerDTO crearAlquilerDTO)
        {

            if (_context.Alquileres == null || _context.Herramientas == null)
            {
                _logger.LogError("Error: La tabla no existe.");
                return StatusCode(500, "Error al configurar la base de datos.");
            }


            if (crearAlquilerDTO == null)
            {
                ModelState.AddModelError("CrearAlquilerDTO", "El objeto no puede ser nulo.");
                return BadRequest(new ValidationProblemDetails(ModelState));
            }



            if (crearAlquilerDTO.AlquilerItems == null || crearAlquilerDTO.AlquilerItems.Count == 0)
            {
                ModelState.AddModelError(nameof(crearAlquilerDTO.AlquilerItems), "El alquiler debe contener al menos una herramienta.");
            }
            if (string.IsNullOrWhiteSpace(crearAlquilerDTO.Nombre))
            {
                ModelState.AddModelError(nameof(crearAlquilerDTO.Nombre), "El nombre del cliente es obligatorio.");
            }
            if (string.IsNullOrWhiteSpace(crearAlquilerDTO.Apellido))
            {
                ModelState.AddModelError(nameof(crearAlquilerDTO.Apellido), "El apellido del cliente es obligatorio.");
            }
            if (string.IsNullOrWhiteSpace(crearAlquilerDTO.DireccionEnvio))
            {
                ModelState.AddModelError(nameof(crearAlquilerDTO.DireccionEnvio), "La dirección de envío es obligatoria.");
            }
            if (!crearAlquilerDTO.DireccionEnvio.Contains("Calle")){
                ModelState.AddModelError(nameof(crearAlquilerDTO.DireccionEnvio), "La direccion de envio tiene que contener la palabra Calle.");
            }
            if (crearAlquilerDTO.FechaInicio <= DateTime.Now)
            {
                ModelState.AddModelError(nameof(crearAlquilerDTO.FechaInicio), "La fecha de inicio debe ser futura.");
            }
            if (crearAlquilerDTO.FechaInicio >= crearAlquilerDTO.FechaFin)
            {
                ModelState.AddModelError(nameof(crearAlquilerDTO.FechaFin), "La fecha de fin debe ser posterior a la fecha de inicio.");
            }


            if (ModelState.ErrorCount > 0)
            {
                return BadRequest(new ValidationProblemDetails(ModelState));
            }


            var nombreHerramienta = crearAlquilerDTO.AlquilerItems!.Select(h => h.NombreHerramienta).Distinct().ToList();
            var herramientas = await _context.Herramientas
                .Include(f => f.Fabricante)
                .Where(h => nombreHerramienta.Contains(h.Nombre))
                .ToListAsync();


            TiposMetodosPago metodoPago = TiposMetodosPago.TarjetaCredito;
            if (crearAlquilerDTO.MetodoPago == 0)
            {
                metodoPago = TiposMetodosPago.TarjetaCredito;
            }
            else if (crearAlquilerDTO.MetodoPago == 1)
            {
                metodoPago = TiposMetodosPago.Paypal;
            }
            else if (crearAlquilerDTO.MetodoPago == 2)
            {
                metodoPago = TiposMetodosPago.Cash;
            }
            else
            {
                ModelState.AddModelError("MetodoPago", "El método de pago especificado no es válido. ¡Utilice 0, 1 o 2!");
            }

            var usuario = await _context.Users.FirstOrDefaultAsync(u => u.NombreCliente == crearAlquilerDTO.Nombre && u.ApellidoCliente == crearAlquilerDTO.Apellido);
            if (usuario == null)
            {
                ModelState.AddModelError(nameof(crearAlquilerDTO.Nombre), "El usuario no existe");
            }
            if (ModelState.ErrorCount > 0)
            {
                return BadRequest(new ValidationProblemDetails(ModelState));
            }

            var NuevoAlquiler = new Alquiler
            {
                ApplicationUser = usuario!,
                DireccionEnvio = crearAlquilerDTO.DireccionEnvio!,
                FechaAlquiler = DateTime.Now,
                FechaInicio = crearAlquilerDTO.FechaInicio,
                FechaFin = crearAlquilerDTO.FechaFin,
                MetodoPago = metodoPago,
                AlquilerItems = new List<AlquilerItem>()
            };

            foreach (var itemDTO in crearAlquilerDTO.AlquilerItems!)
            {
                var herramienta = herramientas.FirstOrDefault(h => h.Nombre == itemDTO.NombreHerramienta);
                if (herramienta == null)
                {
                    ModelState.AddModelError("Herramienta","La herramienta no existe.");
                    return BadRequest(new ValidationProblemDetails(ModelState));
                }
                if (itemDTO.Cantidad <= 0)
                {
                    // Esto devolverá el string exacto que tu test espera
                    return BadRequest("Error: La cantidad de una herramienta en el alquiler debe ser mayor a 0.");
                }
                var alquilerItem = new AlquilerItem
                {
                    Herramienta = herramienta,
                    Cantidad = itemDTO.Cantidad,
                    Precio = herramienta.Precio * itemDTO.Cantidad,

                };
                NuevoAlquiler.AlquilerItems.Add(alquilerItem);
            }


            NuevoAlquiler.PrecioTotal = NuevoAlquiler.AlquilerItems.Sum(ai => ai.Precio);
            NuevoAlquiler.Periodo = (int)(NuevoAlquiler.FechaFin - NuevoAlquiler.FechaInicio).TotalDays;

            _context.Alquileres.Add(NuevoAlquiler);
            await _context.SaveChangesAsync();

            var alquilerDetalleDTO = new AlquilerDetalleDTO(
                usuario!.NombreCliente,
                usuario.ApellidoCliente,
                NuevoAlquiler.DireccionEnvio!,
                NuevoAlquiler.FechaAlquiler,
                NuevoAlquiler.AlquilerItems.Sum(ai => ai.Precio),
                NuevoAlquiler.FechaInicio,
                NuevoAlquiler.FechaFin,
                NuevoAlquiler.Id,
                NuevoAlquiler.AlquilerItems.Select(oi => new AlquilerItemDTO(
                    oi.Herramienta.Nombre,
                    oi.Herramienta.Material,
                    oi.Herramienta.Precio,
                    oi.Cantidad
                    )).ToList()
                

            );

            // FIX 5: Añadir el 'id' del nuevo alquiler a la respuesta 'CreatedAtAction'.
            return CreatedAtAction(nameof(GetAlquilerDetalle), new { id = NuevoAlquiler.Id }, alquilerDetalleDTO);
        }
    }
}