using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AppForSEII2526.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HerramientaController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly ILogger<HerramientaController> _logger;

        public HerramientaController(ApplicationDBContext context, ILogger<HerramientaController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        [Route("[action]")])]
        [ProduceResponseType(typeof(IList<Herramienta>), StatusCodes.Status200OK)]))]
        public asynv Task<IActionResult> GetAllHerramientas()
        {
            IList<Herramienta> herramientas = await _context.Herramientas.ToListAsync();
              return Ok(herramientas);
        }
    }
}
