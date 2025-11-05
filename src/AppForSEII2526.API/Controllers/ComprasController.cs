using AppForSEII2526.API.DTOs;
using AppForSEII2526.API.Models;
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
        public async Task<IActionResult> GetCompraDetalle(int id)
        {
            if (_context.Compras == null)
            {
                _logger.LogError("Error: La tabla no existe.");
                return NotFound();
            }

            var compra = await _context.Compras
                 .Where(c => c.Id == id)
                     .Include(c => c.CompraItems)
                         .ThenInclude(ci => ci.Herramienta)
                             .ThenInclude(h => h.Fabricante)

                 .Select(c => new CompraDetalleDTO(
                     //c.Id,
                     c.ApplicationUser.NombreCliente,
                     c.ApplicationUser.ApellidoCliente,
                     c.DireccionEnvio,
                     c.PrecioTotal,
                     c.FechaCompra,
                     c.CompraItems

                         .Select(ci => new CompraItemDTO(
                             ci.HerramientaId,
                             ci.Herramienta.Nombre,
                             ci.Herramienta.Material,
                             ci.Precio,
                             ci.Descripcion,
                             ci.Cantidad)).ToList<CompraItemDTO>()))

                 .FirstOrDefaultAsync();
            if (compra == null)
            {
                _logger.LogError($"No se encontró la compra con ID {id}.");
                return NotFound();
            }
            return Ok(compra);

        }

       [HttpPost]
       [Route("Crear-Compra")]
       [ProducesResponseType(typeof(CompraDetalleDTO), (int)HttpStatusCode.Created)]
       [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
       [ProducesResponseType(typeof(string), (int)HttpStatusCode.Conflict)]
       public async Task<IActionResult> CrearCompra(CrearCompraDTO crearCompraDTO)
        {
            if (_context.Compras == null || _context.Herramientas == null)
            {
                _logger.LogError("Error: Faltan DbSets.");
                return StatusCode(500, "Error interno");
            }

            if (crearCompraDTO == null)
            {
                ModelState.AddModelError("CrearCompraDTO", "El objeto no puede ser nulo.");
            }

            if (crearCompraDTO.CompraItems == null || crearCompraDTO.CompraItems.Count == 0)
                ModelState.AddModelError("CompraItems", "Error: La compra debe contener al menos un ítem.");

            if (crearCompraDTO.NombreCliente == null)
                ModelState.AddModelError("Nombre del Cliente", "Error: El nombre del cliente no puede ser nulo, es obligatorio.");

            if (crearCompraDTO.ApellidoCliente == null)
                ModelState.AddModelError("Apellido del cliente", "Error: El apellido del cliente no puede ser nulo, es obligatorio.");

            if (crearCompraDTO.DireccionEnvio == null)
                ModelState.AddModelError("Dirección de envío", "Error: La dirección de envío no puede ser nula, es obligatoria.");

            
            TiposMetodosPago metodoPago;
            if (crearCompraDTO.MetodoPagoId == 0)
            {
                metodoPago = TiposMetodosPago.TarjetaCredito;
            }
            else if (crearCompraDTO.MetodoPagoId == 1)
            {
                metodoPago = TiposMetodosPago.Paypal;
            }
            else if (crearCompraDTO.MetodoPagoId == 2)
            {
                metodoPago = TiposMetodosPago.Cash;
            }
            else
            {
                ModelState.AddModelError("MetodoPago", "El método de pago especificado no es válido. ¡Utilice 0, 1 o 2!");
                return ValidationProblem(ModelState);
            }



            //buscamos el usuario
            var usuario = await _context.Users.FirstOrDefaultAsync(u => u.NombreCliente == crearCompraDTO.NombreCliente && u.ApellidoCliente == crearCompraDTO.ApellidoCliente);
            if (usuario == null)
                if (usuario == null)
                {
                    _logger.LogWarning($"Usuario '{crearCompraDTO.NombreCliente}' no encontrado.");
                    return NotFound($"El usuario '{crearCompraDTO.NombreCliente}' no existe.");
                }


            if (ModelState.ErrorCount > 0)
                return BadRequest(new ValidationProblemDetails(ModelState));

            var nombreHerramienta = crearCompraDTO.CompraItems.Select(h => h.NombreHerramienta).Distinct().ToList();

            var herramientas = await _context.Herramientas
                .Include(f => f.Fabricante)
                .Where(h => nombreHerramienta.Contains(h.Nombre))
                .ToListAsync();

            var compraNueva = new Compra
            {
                DireccionEnvio = crearCompraDTO.DireccionEnvio,
                FechaCompra = DateTime.UtcNow,
                PrecioTotal = 0m,
                MetodoPago = metodoPago,
                CompraItems = new List<CompraItem>(),
                ApplicationUser = usuario
            };

            foreach (var itemDTO in crearCompraDTO.CompraItems)
            {

                if (itemDTO == null)
                {
                    ModelState.AddModelError("Items", "Error: El ítem de compra no puede ser nulo.");
                    continue;
                }

                if (itemDTO.Cantidad == null)
                {
                    ModelState.AddModelError("CompraItems", $"La cantidad para la herramienta {itemDTO.NombreHerramienta} es un campo obligatorio.");
                }
                if (itemDTO.Cantidad <= 0)
                {
                    ModelState.AddModelError("CompraItems", $"La cantidad para la herramienta {itemDTO.NombreHerramienta} debe ser mayor a cero.");
                }
                if (itemDTO.DescripcionHerramienta == null)
                {
                    ModelState.AddModelError("CompraItems", $"La descripción para la herramienta {itemDTO.NombreHerramienta} no puede ser nula.");
                }

                var herramienta = herramientas.FirstOrDefault(h => h.Nombre == itemDTO.NombreHerramienta);

                if (herramienta == null)
                {
                    ModelState.AddModelError("Items", $"La herramienta '{itemDTO.NombreHerramienta}' no existe.");
                    return ValidationProblem(ModelState);
                }
                else
                {
                    decimal precioUnitario = herramienta.Precio;
                    int cantidad = itemDTO.Cantidad;

                    var itemCompra = new CompraItem
                    {
                        HerramientaId = herramienta.Id,
                        Cantidad = itemDTO.Cantidad,
                        Descripcion = itemDTO.DescripcionHerramienta,
                        Precio = precioUnitario * cantidad,
                        Herramienta = herramienta,
                        Compra = compraNueva
                    };

                    compraNueva.CompraItems.Add(itemCompra);
                }
            }
            if (ModelState.ErrorCount > 0)
                return BadRequest(new ValidationProblemDetails(ModelState));

            compraNueva.PrecioTotal = compraNueva.CompraItems.Sum(ci => ci.Precio);


           

            _context.Compras.Add(compraNueva);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar la compra en la base de datos.");
                ModelState.AddModelError("Database", "Error al guardar la compra en la base de datos.");
                return Conflict("Error al guardar la compra." + ex.Message);
            }

            var compraDTOResp = new CompraDetalleDTO(
                compraNueva.ApplicationUser.NombreCliente,
                compraNueva.ApplicationUser.ApellidoCliente,
                compraNueva.DireccionEnvio,
                compraNueva.PrecioTotal,
                compraNueva.FechaCompra,
                compraNueva.CompraItems.Select(ci => new CompraItemDTO(
                    ci.HerramientaId,
                    ci.Herramienta.Nombre,
                    ci.Herramienta.Material,
                    ci.Herramienta.Precio,
                    ci.Descripcion,
                    ci.Cantidad
                    )).ToList()
                    );

            return Ok(compraDTOResp);



        }
    }
}
