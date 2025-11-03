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
        public async Task<IActionResult> GetCompraDetalle()
        {
            if (_context.Compras == null)
            {
                _logger.LogError("Error: La tabla no existe.");
                return NotFound();
            }

            IList<CompraDetalleDTO> compraDetalles = await _context.Compras
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
                ModelState.AddModelError(nameof(crearCompraDTO.CompraItems), "La compra debe contener al menos un ítem.");

            if (crearCompraDTO.Nombre == null)
                ModelState.AddModelError(nameof(crearCompraDTO.Nombre), "El nombre del cliente no puede ser nulo.");

            if (crearCompraDTO.Apellido == null)
                ModelState.AddModelError(nameof(crearCompraDTO.Apellido), "El apellido del cliente no puede ser nulo.");

            if (crearCompraDTO.DireccionEnvio == null)
                ModelState.AddModelError(nameof(crearCompraDTO.DireccionEnvio), "La dirección de envío no puede ser nula.");

            if (crearCompraDTO.MetodoPagoId == null)
                ModelState.AddModelError(nameof(crearCompraDTO.MetodoPagoId), "El método de pago no puede ser nulo.");


            /*
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
            */


            //buscamos el usuario
            var usuario = await _context.Users.FirstOrDefaultAsync(u => u.NombreCliente == crearCompraDTO.Nombre && u.ApellidoCliente == crearCompraDTO.Apellido);
            if (usuario == null)
                ModelState.AddModelError(nameof(crearCompraDTO.Nombre), "El usuario no existe");


            if (ModelState.ErrorCount > 0)
                return BadRequest(new ValidationProblemDetails(ModelState));

            var herramientasNombre = crearCompraDTO.CompraItems.Select(ci => ci.NombreHerramienta).ToList();
            var herramientas = await _context.Herramientas
                .Include(h => h.Fabricante)
                .Where(h => herramientasNombre.Contains(h.Nombre))
                .ToListAsync();

            var compraNueva = new Compra(usuario, crearCompraDTO.DireccionEnvio, DateTime.Today, crearCompraDTO.PrecioTotal, crearCompraDTO.MetodoPagoId, new List<CompraItem>());

            foreach (var itemDTO in crearCompraDTO.CompraItems)
            {
                var herramienta = herramientas.FirstOrDefault(h => h.Nombre == itemDTO.NombreHerramienta);
                if (herramienta = null)
                {
                    ModelState.AddModelError("CompraItems", $"La herramienta {itemDTO.NombreHerramienta} no existe.");
                    continue;

                }

                if (itemDTO.Cantidad == null)
                {
                    ModelState.AddModelError("CompraItems", $"La cantidad para la herramienta {itemDTO.NombreHerramienta} es un campo obligatorio.");
                }

                if (itemDTO.DescripcionHerramienta == null)
                {
                    ModelState.AddModelError("CompraItems", $"La descripción para la herramienta {itemDTO.NombreHerramienta} no puede ser nula.");
                }

                if (itemDTO.Cantidad <= 0)
                {
                    ModelState.AddModelError("CompraItems", $"La cantidad para la herramienta {itemDTO.NombreHerramienta} debe ser mayor a cero.");
                }
                else
                {
                    compraNueva.PrecioTotal += herramienta.Precio * itemDTO.Cantidad;
                    compraNueva.CompraItems.Add(new CompraItem(itemDTO.Cantidad, itemDTO.PrecioHerramienta, herramienta, itemDTO.DescripcionHerramienta, herramienta, compraNueva));
                }


                if (ModelState.ErrorCount > 0)
                    return BadRequest(new ValidationProblemDetails(ModelState));

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
                    usuario.NombreCliente,
                    usuario.ApellidoCliente,
                    compraNueva.DireccionEnvio,
                    compraNueva.PrecioTotal,
                    compraNueva.FechaCompra,
                    compraNueva.CompraItems.Select(ci => new CompraItemDTO(
                        ci.Herramienta.Nombre,
                        ci.Herramienta.Material,
                        ci.Herramienta.Precio,
                        ci.Descripcion,
                        ci.Cantidad
                        )).ToList<CompraItemDTO>());

                return CreatedAtAction(nameof(GetCompraDetalle), new { id = compraNueva.Id }, compraDTOResp);



            }
        }
    } }



           
