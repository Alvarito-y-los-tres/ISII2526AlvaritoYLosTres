using AppForSEII2526.API.Controllers;
using AppForSEII2526.API.DTOs;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppForSEII2526.UT.HerramientasController_test
{
    public class GetHerramientaParaComprar_test: AppForSEII25264SqliteUT
    {
        public GetHerramientaParaComprar_test()
        {
            var fabricante = new List<Fabricante>()
            {
                new Fabricante("Ferretería Lopez"),
                new Fabricante("Feterrería García"),
                new Fabricante("Ferretería Ruiz")
            };

            var herramienta = new List<Herramienta>()
            {
                new Herramienta ("Martillo", "Madera", 35, 3, fabricante[0]),
                new Herramienta ("Alicates", "Acero", 12, 1, fabricante[1]),
                new Herramienta ("Tijeras", "Plastico", 3, 1, fabricante[2])
            };

            // ApplicationUser user = new ApplicationUser("Ana", "López Marín", "661234567", "analala40.all@gmail.com");

            //var compra = new Compra("Calle Tejares", 12/06/2025 14:04:12, 35, TiposMetodosPago.TarjetaCredito);

            _context.AddRange(fabricante);
            _context.AddRange(herramienta);
            _context.SaveChanges();
        }

        public static IEnumerable<object[]> TestCasesFor_GetHerramientasParaComprar_OK()
        {
            var herramientaDTOs = new List<HerramientasParaComprarDTO>()
            {
                new HerramientasParaComprarDTO("Martillo", "Madera", 35, "Ferretería Lopez"),
                new HerramientasParaComprarDTO("Alicates", "Acero", 12, "Feterrería García"),
                new HerramientasParaComprarDTO("Tijeras", "Plastico", 3, "Ferretería Ruiz")
            };

            var herramientaDTOsTC1 = new List<HerramientasParaComprarDTO>() { herramientaDTOs[1], herramientaDTOs[2] }
            .OrderBy(h => h.Nombre).ToList();

            var herramientaDTOsTC2 = new List<HerramientasParaComprarDTO>() { herramientaDTOs[1] };
            var herramientaDTOsTC3 = new List<HerramientasParaComprarDTO>() { herramientaDTOs[2] };
            var herramientaDTOsTC4 = new List<HerramientasParaComprarDTO>() { herramientaDTOs[0], herramientaDTOs[1], herramientaDTOs[2] }
            .OrderBy(h => h.Nombre).ToList();

            var allTests = new List<object[]>
            {
                new object[] { null, null, herramientaDTOsTC1 },
                new object[] { "Acero", null, herramientaDTOsTC2 },
                new object[] { null, 10, herramientaDTOsTC3 },
            };
            return allTests;
        }


        [Theory]
        [MemberData(nameof(TestCasesFor_GetHerramientasParaComprar_OK))]
        [Trait("Database", "WithoutFixture")]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task GetHerramientaParaComprar_OK_test(string? material, decimal? precio,
            IList<HerramientasParaComprarDTO> expectedHerramienta)
        {
            // Arrange
            var controller = new HerramientaController(_context, null);

            // Act
            var result = await controller.GetHerramientaParaComprar(material, precio);

            //Assert
            //we check that the response type is OK 
            var okResult = Assert.IsType<OkObjectResult>(result);
            //and obtain the list of herramientas
            var herramientaDTOsActual = Assert.IsType<List<HerramientasParaComprarDTO>>(okResult.Value);
            Assert.Equal(expectedHerramienta, herramientaDTOsActual);

        }
        }


    }

