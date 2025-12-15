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
            _logger.LogInformation("Iniciando búsqueda de ofertas. Filtros -> Fabricante: {Fabricante}, Precio Máximo: {Precio}",
                fabricante ?? "Todos",
                precio.HasValue ? precio.Value.ToString() : "Sin límite");

            try
            {
                IList<HerramientasParaOfertarDTO> selectHerramienta = await _context.Herramientas
                    .Include(h => h.Fabricante)
                    .Where(h => (fabricante == null || h.Fabricante.Nombre == fabricante) &&
                                (precio == null || h.Precio <= precio))
                    .Select(h => new HerramientasParaOfertarDTO(h.Nombre, h.Material, h.Fabricante.Nombre, h.Precio))
                    .ToListAsync();

                if (selectHerramienta.Count == 0)
                {
                    _logger.LogWarning("La búsqueda no arrojó resultados para Fabricante: {Fabricante} y Precio: {Precio}", fabricante, precio);
                }
                else
                {
                    _logger.LogInformation("Búsqueda exitosa. Se han recuperado {Count} herramientas.", selectHerramienta.Count);
                }

                return Ok(selectHerramienta);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error crítico al consultar herramientas para oferta en la base de datos.");


                return StatusCode(500, "Ocurrió un error interno al procesar su solicitud.");
            }
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
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)] 
        public async Task<IActionResult> GetHerramientasParaAlquilar(string? nombre, string? material)
        {
      
            _logger.LogInformation("Iniciando búsqueda de herramientas para alquilar. Filtros -> Nombre: {Nombre}, Material: {Material}",
                nombre ?? "Todos", material ?? "Todos");

            if (_context.Herramientas == null)
            {
                _logger.LogCritical("CRITICAL: La tabla 'Herramientas' no existe o es nula en el contexto de base de datos.");
                return StatusCode(500, "Error interno al acceder a la base de datos.");
            }

            var herramientasAlquilar = await _context.Herramientas
                .Include(h => h.Fabricante)
                .Where(h => (nombre == null || h.Nombre == nombre) && (material == null || h.Material == material))
                .Select(h => new HerramientaAlquilarDTO(
                    h.Nombre,
                    h.Material,
                    h.Fabricante.Nombre,
                    h.Precio))
                .ToListAsync();

            if (herramientasAlquilar.Count == 0)
            {
                
                _logger.LogWarning("La búsqueda se completó sin resultados para Nombre: {Nombre} y Material: {Material}", nombre, material);
            }
            else
            {
                _logger.LogInformation("Búsqueda finalizada con éxito. Se devolvieron {Count} herramientas.", herramientasAlquilar.Count);
            }

            return Ok(herramientasAlquilar);
        }
        [HttpGet]
        [Route("Para-Reparar")]
        [ProducesResponseType(typeof(IList<HerramientaRepararDTO>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetHerramientasParaReparar(string? nombre, float? tiempoReparacion)
        {
            // [LOG] Registramos los parámetros de entrada.
            // Usamos "Todos" o "Sin límite" para que el log sea legible si los valores son null.
            _logger.LogInformation("Iniciando búsqueda 'Para-Reparar'. Filtros -> Nombre: {NombreFiltro}, TiempoReparacionMax: {TiempoFiltro}", 
                nombre ?? "Todos", 
                tiempoReparacion.HasValue ? tiempoReparacion.Value.ToString() : "Sin límite");

            if (_context.Herramientas == null)
            {
                _logger.LogError("Error crítico: El DbSet de Herramientas es nulo.");
                return StatusCode(500, "Error interno del servidor");
            }

            var herramientasReparar = await _context.Herramientas
                .Include(h => h.Fabricante)
                // La lógica de filtrado se mantiene igual
                .Where(h => (nombre == null || h.Nombre == nombre) && 
                            (tiempoReparacion == null || h.TiempoReparacion <= tiempoReparacion))
                .Select(h => new HerramientaRepararDTO(
                    h.Nombre,
                    h.Material,
                    h.Fabricante.Nombre,
                    h.Precio,
                    h.TiempoReparacion))
                .ToListAsync();

            // [LOG] Registramos cuántos elementos devolvió la base de datos
            _logger.LogInformation("Búsqueda completada. Se encontraron {Count} herramientas que coinciden con los criterios.", herramientasReparar.Count);

            return Ok(herramientasReparar);
        }

    }
}
