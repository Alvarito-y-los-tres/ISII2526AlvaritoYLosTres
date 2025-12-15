using AppForSEII2526.API.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AppForSEII2526.API.Models;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.EntityFrameworkCore; // Necesario para Include y Async

namespace AppForSEII2526.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReparacionesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ReparacionesController> _logger;

        public ReparacionesController(ApplicationDbContext context, ILogger<ReparacionesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        [Route("Reparacion-Detalle")]
        [ProducesResponseType(typeof(ReparacionDetalleDTO), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetReparacionDetalle(int ID)
        {
            // [LOG] Inicio del proceso
            _logger.LogInformation("Iniciando solicitud GetReparacionDetalle para el ID: {ReparacionId}", ID);

            if (_context.Reparaciones == null)
            {
                _logger.LogError("Error crítico: La tabla Reparaciones no existe en el contexto.");
                return NotFound();
            }

            var reparacionDetalles = await _context.Reparaciones
                .Where(r => r.Id == ID)
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
                        ri.Descripcion,
                        ri.Cantidad,
                        ri.Precio,
                        ri.ReparacionId
                    )).ToList()
                ))
                .FirstOrDefaultAsync();

            if (reparacionDetalles == null)
            {
                // [LOG] No encontrado
                _logger.LogWarning("No se encontraron reparaciones con el ID: {ReparacionId}", ID);
                return NotFound();
            }

            // [LOG] Éxito
            _logger.LogInformation("Reparación recuperada exitosamente para el cliente {Nombre} {Apellido}. Total Items: {TotalItems}", 
                reparacionDetalles.NombreCLiente, 
                reparacionDetalles.ApellidoCliente, 
                reparacionDetalles.items.Count);

            return Ok(reparacionDetalles);
        }

        //post
        [HttpPost]
        [Route("Crear-Reparacion")]
        [ProducesResponseType(typeof(ReparacionDetalleDTO), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Conflict)]
        public async Task<IActionResult> CrearReparacion([FromBody] CrearReparacionDTO dto)
        {
            // [LOG] Inicio de creación
            _logger.LogInformation("Iniciando proceso de CrearReparacion para cliente: {Nombre} {Apellido}", dto?.NombreCliente, dto?.ApellidoCliente);

            if (_context.Reparaciones == null || _context.Herramientas == null)
            {
                _logger.LogError("Error crítico: Faltan DbSets (Reparaciones o Herramientas).");
                return StatusCode(500, "Error interno");
            }

            if (dto == null)
            {
                _logger.LogWarning("Intento de crear reparación con DTO nulo.");
                ModelState.AddModelError("CrearReparacionDTO", "El objeto no puede ser nulo.");
                return BadRequest(new ValidationProblemDetails(ModelState));
            }

            if(dto.Telefono != null && !dto.Telefono.StartsWith("+34"))
            {
                _logger.LogWarning("Validación fallida: El teléfono {Telefono} no empieza por +34.", dto.Telefono);
                ModelState.AddModelError("CrearReparacionDTO", "Error,el telefono debe empezar por +34");
                return BadRequest(new ValidationProblemDetails(ModelState));
            }

            if (dto.Items == null || !dto.Items.Any())
                ModelState.AddModelError("Items", "Debe incluir al menos una herramienta para reparar.");

            if (string.IsNullOrWhiteSpace(dto.NombreCliente))
                ModelState.AddModelError("NombreCliente", "El nombre del cliente es obligatorio.");

            if (string.IsNullOrWhiteSpace(dto.ApellidoCliente))
                ModelState.AddModelError("ApellidoCliente", "El apellido del cliente es obligatorio.");

            if (dto.FechaEntrega < DateTime.UtcNow.Date)
                ModelState.AddModelError("FechaEntrega", "La fecha de entrega no puede ser en el pasado.");

            // Validar método de pago
            if (!Enum.IsDefined(typeof(TiposMetodosPago), dto.MetodoPago))
            {
                _logger.LogWarning("Método de pago inválido recibido: {MetodoPago}", dto.MetodoPago);
                ModelState.AddModelError("MetodoPago", "Método de pago inválido.");
                return BadRequest(new ValidationProblemDetails(ModelState));
            }
            var metodoPago = (TiposMetodosPago)dto.MetodoPago;

            if (ModelState.ErrorCount > 0)
            {
                _logger.LogWarning("Falló la validación del modelo inicial. Errores: {ErrorCount}", ModelState.ErrorCount);
                return BadRequest(new ValidationProblemDetails(ModelState));
            }

            // Buscar el usuario por nombre y apellido
            _logger.LogInformation("Buscando usuario '{Nombre} {Apellido}' en la base de datos...", dto.NombreCliente, dto.ApellidoCliente);
            
            var usuario = await _context.Users
                .FirstOrDefaultAsync(u => u.NombreCliente == dto.NombreCliente && u.ApellidoCliente == dto.ApellidoCliente);

            if (usuario == null)
            {
                _logger.LogWarning("Usuario '{Nombre} {Apellido}' no encontrado.", dto.NombreCliente, dto.ApellidoCliente);
                return NotFound($"El usuario '{dto.NombreCliente} {dto.ApellidoCliente}' no existe.");
            }

            // Buscar todas las herramientas mencionadas
            var nombresHerramientas = dto.Items.Select(i => i.NombreHerramienta).Distinct().ToList();
            
            _logger.LogInformation("Verificando existencia de {Count} herramientas solicitadas.", nombresHerramientas.Count);

            var herramientas = await _context.Herramientas
                .Include(h => h.Fabricante)
                .Where(h => nombresHerramientas.Contains(h.Nombre))
                .ToListAsync();

            // Validar que todas las herramientas existan
            foreach (var item in dto.Items)
            {
                if (!herramientas.Any(h => h.Nombre == item.NombreHerramienta))
                {
                    _logger.LogWarning("Herramienta solicitada no existe: {HerramientaName}", item.NombreHerramienta);
                    ModelState.AddModelError("Items", $"La herramienta '{item.NombreHerramienta}' no existe.");
                }
            }

            if (ModelState.ErrorCount > 0)
                return BadRequest(new ValidationProblemDetails(ModelState));

            // Crear items de reparación y calcular precio total
            decimal precioTotal = 0;
            var itemsReparacion = new List<ReparacionItem>();

            foreach (var item in dto.Items)
            {
                var herramienta = herramientas.First(h => h.Nombre == item.NombreHerramienta);
                decimal subtotal = herramienta.Precio * item.Cantidad;
                precioTotal += subtotal;

                itemsReparacion.Add(new ReparacionItem
                {
                    Herramienta = herramienta,
                    Descripcion = item.DescripcionProblema,
                    Cantidad = item.Cantidad,
                    Precio = herramienta.Precio,
                    ReparacionId = item.ReparacionId,
                });
            }

            // [LOG] Información de cálculo
            _logger.LogInformation("Cálculo completado. Total estimado: {PrecioTotal}. Cantidad de Items: {Count}", precioTotal, itemsReparacion.Count);

            // Crear reparación con fecha de recogida = un día por cada herramienta
            var reparacion = new Reparacion
            {
                FechaEntrega = dto.FechaEntrega,
                MetodoPago = metodoPago,
                PrecioTotal = precioTotal,
                ApplicationUser = usuario,
                ItemsReparacion = itemsReparacion,
                FechaRecogida = dto.FechaEntrega.AddDays(dto.Items.Count) 
            };

            _context.Reparaciones.Add(reparacion);

            try
            {
                await _context.SaveChangesAsync();
                // [LOG] Guardado exitoso
                _logger.LogInformation("Reparación guardada correctamente en BD con ID generado (si aplica). Fecha Recogida: {FechaRecogida}", reparacion.FechaRecogida);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Excepción al guardar la reparación en la base de datos.");
                return Conflict("Error al guardar la reparación: " + ex.Message);
            }

            // DTO de salida con precio calculado
            var reparacionDTOResp = new ReparacionDetalleDTO(
                usuario.NombreCliente,
                usuario.ApellidoCliente,
                precioTotal,
                reparacion.FechaEntrega,
                reparacion.FechaRecogida,
                itemsReparacion.Select(ri => new ReparacionItemDTO(
                    ri.Herramienta.Nombre,
                    ri.Descripcion,
                    ri.Cantidad,
                    ri.Herramienta.Precio,
                    ri.ReparacionId
                )).ToList()
            );

            return StatusCode(201, reparacionDTOResp);
        }
    }
}