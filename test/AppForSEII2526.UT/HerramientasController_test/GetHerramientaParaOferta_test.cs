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
                new Fabricante ("Ferreteía López"),
                new Fabricante ("Ferreteía García"),
                new Fabricante ("Ferreteía Ruiz")
            };

            var herramientas = new List<Herramienta>
            {
                new Herramienta ("Martillo", "Madera", 35, 3, fabricante[0]),
                new Herramienta ("Alicates", "Acero", 12, 1, fabricante[1]),
                new Herramienta ("Tijeras", "Plastico", 3, 1, fabricante[2])
            };

            //ApplicationUser user = new ApplicationUser("Álvaro", "Cano Andrés", "600123456", "alvaro@gmail.com");

            _context.AddRange(fabricante);
            _context.AddRange(herramientas);
            _context.SaveChanges();

        }

        public static IEnumerable<object?[]> TestCasesFor_GetHerramientasParaOferta_OK()
        {
            var herramientasDTOs = new List<HerramientasParaOfertarDTO>
            {
                new HerramientasParaOfertarDTO ("Martillo", "Madera", "Ferreteía López", 35),
                new HerramientasParaOfertarDTO ("Alicates", "Acero", "Ferreteía García", 12),
                new HerramientasParaOfertarDTO ("Tijeras", "Plastico", "Ferreteía Ruiz", 3)
            };

            var herramientasDTOs_TC1 = new List<HerramientasParaOfertarDTO> { herramientasDTOs[1], herramientasDTOs[2] }
            .OrderBy(h => h.Nombre).ToList();

            var herramientasDTOs_TC2 = new List<HerramientasParaOfertarDTO> { herramientasDTOs[1] };
            var herramientasDTOs_TC3 = new List<HerramientasParaOfertarDTO> { herramientasDTOs[2] };

            var herramientasDTOs_TC4 = new List<HerramientasParaOfertarDTO> { herramientasDTOs[0], herramientasDTOs[1], herramientasDTOs[2] }
            .OrderBy(h => h.Nombre).ToList();

            var allTestCases = new List<object?[]>
            {
                new object[] { null, null, herramientasDTOs_TC1 },
                new object[] { "Alicates", null, herramientasDTOs_TC2 },
                new object[] { null, "Plastico", herramientasDTOs_TC3 }
            };
            return allTestCases;
        }

        [Theory]
        [MemberData(nameof(TestCasesFor_GetHerramientasParaOferta_OK))]
        [Trait("Database", "WithoutFixture")]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task GetHerramientaParaOferta_OK_test(string? fabricante, decimal? precio, IList<HerramientasParaOfertarDTO>herramientasDTOEsperado)
        {
            var controller = new HerramientaController(_context, null);   

            var result = await controller.GetHerramientaParaOferta(fabricante, precio);

            var okResult = result as OkObjectResult;

            var herramientasDTOActual = Assert.IsType<IList<HerramientasParaOfertarDTO>>(okResult.Value);
            Assert.Equal(herramientasDTOEsperado, herramientasDTOActual);
        }
    }
}
