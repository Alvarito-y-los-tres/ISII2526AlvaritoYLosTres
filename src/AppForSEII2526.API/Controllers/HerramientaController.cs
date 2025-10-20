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
        [Route("Para-Comprar")]
        [ProducesResponseType(typeof(IList<HerramientasParaComprarDTO>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult> GetHerramientaParaComprar(string? material, decimal? precio)
        {
            IList<HerramientasParaComprarDTO> selectHerramienta = await _context.Herramientas
                .Include(h => h.Fabricante)
                .Include(h => h.CompraItems).ThenInclude(pi => pi.Compra)
                .Where(h => (material == null || h.Material == material) &&
                            (precio == null || h.Precio <= precio))
                .Select(h => new HerramientasParaComprarDTO(h.Nombre, h.Material, h.Precio, h.Fabricante.Nombre))
                .ToListAsync();
            return Ok(selectHerramienta);
        }

    }
}
