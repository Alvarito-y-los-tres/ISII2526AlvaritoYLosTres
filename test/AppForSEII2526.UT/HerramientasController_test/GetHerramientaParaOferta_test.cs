using AppForSEII2526.API.Controllers;
using AppForSEII2526.API.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppForSEII2526.UT.HerramientasController_test
{
    public class GetHerramientaParaOferta_test: AppForSEII25264SqliteUT
    {
        public GetHerramientaParaOferta_test()
        {
            var fabricante = new List<Fabricante>
            {
                new Fabricante ("Ferretería López"),
                new Fabricante ("Ferretería García"),
                new Fabricante ("Ferretería Ruiz")
            };

            var herramientas = new List<Herramienta>
            {
                new Herramienta ("Martillo", "Madera", 35, 3, fabricante[0]),
                new Herramienta ("Alicates", "Acero", 12, 1, fabricante[1]),
                new Herramienta ("Tijeras", "Plastico", 3, 1, fabricante[2])
            };

            _context.AddRange(fabricante);
            _context.AddRange(herramientas);
            _context.SaveChanges();

        }

        public static IEnumerable<object?[]> TestCasesFor_GetHerramientasParaOferta_OK()
        {
            var herramientasDTOs = new List<HerramientasParaOfertarDTO>
            {
                new HerramientasParaOfertarDTO ("Martillo", "Madera", "Ferretería López", 35),
                new HerramientasParaOfertarDTO ("Alicates", "Acero", "Ferretería García", 12),
                new HerramientasParaOfertarDTO ("Tijeras", "Plastico", "Ferretería Ruiz", 3)
            };

            var herramientasDTOs_TC1 = new List<HerramientasParaOfertarDTO> { herramientasDTOs[0], herramientasDTOs[1], herramientasDTOs[2] };

            var herramientasDTOs_TC2 = new List<HerramientasParaOfertarDTO> { herramientasDTOs[1] };
            var herramientasDTOs_TC3 = new List<HerramientasParaOfertarDTO> { herramientasDTOs[2] };

            var allTestCases = new List<object?[]>
            {
                new object[] { null, null, herramientasDTOs_TC1 },
                new object[] { "Ferretería García", null, herramientasDTOs_TC2 },
                new object[] { null, 5, herramientasDTOs_TC3 }
            };
            return allTestCases;
        }

        [Theory]
        [MemberData(nameof(TestCasesFor_GetHerramientasParaOferta_OK))]
        [Trait("Database", "WithoutFixture")]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task GetHerramientaParaOferta_OK_test(string? fabricante, int? precio, IList<HerramientasParaOfertarDTO>herramientasDTOEsperado)
        {
            var controller = new HerramientaController(_context, null);   

            var result = await controller.GetHerramientaParaOferta(fabricante, precio);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var herramientasDTOActual = Assert.IsType<List<HerramientasParaOfertarDTO>>(okResult.Value);
            Assert.Equal(herramientasDTOEsperado, herramientasDTOActual);
        }
    }
}
