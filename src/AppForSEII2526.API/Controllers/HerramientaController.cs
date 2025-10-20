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
        public async Task<ActionResult> GetHerramientaParaOferta(string? fabricante, decimal? precio)
        {
            IList<HerramientasParaOfertarDTO> selectHerramienta = await _context.Herramientas
                .Include(h => h.Fabricante)
                .Include(h => h.OfertaItems).ThenInclude(pi => pi.Oferta)
                .Where(h => (fabricante == null || h.Fabricante.Nombre == fabricante) &&
                            (precio == null || h.Precio <= precio))
                .Select(h => new HerramientasParaOfertarDTO(h.Nombre, h.Material, h.Fabricante.Nombre, h.Precio))
                .ToListAsync();
            return Ok(selectHerramienta);
        }
    }
}
