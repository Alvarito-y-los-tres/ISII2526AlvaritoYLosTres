using AppForSEII2526.API.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace AppForSEII2526.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HerramientaController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HerramientaController> _logger;

        public HerramientaController(ApplicationDbContext context, ILogger<HerramientaController> logger)
        {
            _context = context;
            _logger = logger;
            _logger.LogInformation("HerramientaController initialized.");
		}

        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(IList<Herramienta>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAllHerramientas()
        {

			IList<Herramienta> herramientas = await _context.Herramientas.ToListAsync();
              return Ok(herramientas);
        }

        [HttpGet]
        [Route("Para-Oferta")]
        [ProducesResponseType(typeof(IList<HerramientasParaOfertarDTO>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetHerramientaParaOferta(string? fabricante, decimal? precio)
        {
            IList<HerramientasParaOfertarDTO> selectHerramienta = await _context.Herramientas
                .Include(h => h.Fabricante)
                .Where(h => (fabricante == null || h.Fabricante.Nombre == fabricante) &&
                            (precio == null || h.Precio <= precio))
                .Select(h => new HerramientasParaOfertarDTO(h.Nombre, h.Material, h.Fabricante.Nombre, h.Precio))
                .ToListAsync();
            return Ok(selectHerramienta);
        }

        [HttpGet]
        [Route("Para-Comprar")]
        [ProducesResponseType(typeof(IList<HerramientasParaComprarDTO>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult> GetHerramientaParaComprar(string? material, decimal? precio)
        {
            _logger.LogInformation(
                "GET Para-Comprar iniciado con filtros -> Material: {Material}, Precio máximo: {Precio}",
                material, precio);

            try
            {
                IList<HerramientasParaComprarDTO> selectHerramienta = await _context.Herramientas
                    .Include(h => h.Fabricante)
                    .Include(h => h.CompraItems)
                        .ThenInclude(pi => pi.Compra)
                    .Where(h => (material == null || h.Material == material) &&
                                (precio == null || h.Precio <= precio))
                    .Select(h => new HerramientasParaComprarDTO(
                        h.Nombre,
                        h.Material,
                        h.Precio,
                        h.Fabricante.Nombre))
                    .ToListAsync();

                if (!selectHerramienta.Any())
                {
                    _logger.LogWarning(
                        "No se encontraron herramientas para comprar con los filtros -> Material: {Material}, Precio: {Precio}",
                        material, precio);
                }
                else
                {
                    _logger.LogInformation(
                        "Se han encontrado {Cantidad} herramientas para comprar",
                        selectHerramienta.Count);
                }

                return Ok(selectHerramienta);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error al obtener herramientas para comprar con filtros -> Material: {Material}, Precio: {Precio}",
                    material, precio);

                return StatusCode((int)HttpStatusCode.InternalServerError,
                    "Error interno al obtener las herramientas");
            }
        }

        [HttpGet]
        [Route("Para-Alquilar")]
        [ProducesResponseType(typeof(IList<HerramientaAlquilarDTO>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetHerramientasParaAlquilar(string? nombre, string? material)
        {
            var herramientasAlquilar = await _context.Herramientas
                .Include(h => h.Fabricante)
                // 👇 LÍNEA CORREGIDA 👇
                .Where(h => (nombre == null || h.Nombre == nombre) && (material == null || h.Material == material))
                .Select(h => new HerramientaAlquilarDTO(
                    h.Nombre,
                    h.Material,
                    h.Fabricante.Nombre,
                    h.Precio))
                .ToListAsync();
            return Ok(herramientasAlquilar);
        }
        [HttpGet]
        [Route("Para-Reparar")]
        [ProducesResponseType(typeof(IList<HerramientaRepararDTO>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetHerramientasParaReparar(string? nombre, float? tiempoReparacion)
        {
            var herramientasReparar = await _context.Herramientas
                .Include(h => h.Fabricante)
                .Where(h => (nombre == null || h.Nombre == nombre) && (tiempoReparacion == null || h.TiempoReparacion <= tiempoReparacion))
                .Select(h => new HerramientaRepararDTO(
                    h.Nombre,
                    h.Material,
                    h.Fabricante.Nombre,
                    h.Precio,
                    h.TiempoReparacion))
                .ToListAsync();
            return Ok(herramientasReparar);
        }

    }
}
