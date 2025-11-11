using AppForSEII2526.API.Controllers;
using AppForSEII2526.API.DTOs;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AppForSEII2526.UT.HerramientasController_test
{
    public class GetHerramientaParaAlquilar_test : AppForSEII25264SqliteUT
    {
        public GetHerramientaParaAlquilar_test()
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

            _context.AddRange(fabricante);
            _context.AddRange(herramientas);
            _context.SaveChanges();
        }

        public static IEnumerable<object?[]> TestCasesFor_GetHerramientasParaAlquilar_OK()
        {
            // FIX 1: Usar el DTO correcto: HerramientaAlquilarDTO
            var herramientasDTOs = new List<HerramientaAlquilarDTO>
      {
        new HerramientaAlquilarDTO ("Martillo", "Madera", "Ferreteía López", 35), // [0]
                new HerramientaAlquilarDTO ("Alicates", "Acero", "Ferreteía García", 12), // [1]
                new HerramientaAlquilarDTO ("Tijeras", "Plastico", "Ferreteía Ruiz", 3)  // [2]
            };

            // (Opcional, pero recomendado) Quito el .OrderBy() porque Assert.Equivalent no lo necesita
            var herramientasDTOs_TC1 = new List<HerramientaAlquilarDTO> { herramientasDTOs[1], herramientasDTOs[2] };

            var herramientasDTOs_TC2 = new List<HerramientaAlquilarDTO> { herramientasDTOs[1] };
            var herramientasDTOs_TC3 = new List<HerramientaAlquilarDTO> { herramientasDTOs[2] };

            // Esta es la lista completa (3 items)
            var herramientasDTOs_TC4 = new List<HerramientaAlquilarDTO> { herramientasDTOs[0], herramientasDTOs[1], herramientasDTOs[2] };

            var allTestCases = new List<object?[]>
      {
                // FIX 2: El caso (null, null) debe esperar la lista completa (TC4)
                new object[] { (string?)null, (string?)null, herramientasDTOs_TC4 },
        new object[] { "Alicates", (string?)null, herramientasDTOs_TC2 },
        new object[] { (string?)null, "Plastico", herramientasDTOs_TC3 }
      };
            return allTestCases;
        }

        [Theory]
        [MemberData(nameof(TestCasesFor_GetHerramientasParaAlquilar_OK))]
        [Trait("Database", "WithoutFixture")]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task GetHerramientaParaAlquilar_OK_test(string? nombre, string? material, IList<HerramientaAlquilarDTO> herramientasDTOEsperado)
        {
            var controller = new HerramientaController(_context, null!);

            var result = await controller.GetHerramientasParaAlquilar(nombre, material);

            var okResult = Assert.IsType<OkObjectResult>(result);

            // FIX 3: Usar IsAssignableFrom para comprobar la interfaz
            var herramientasDTOActual = Assert.IsAssignableFrom<IList<HerramientaAlquilarDTO>>(okResult.Value);

            // FIX 4: Usar Equivalent para ignorar el orden de los elementos
            Assert.Equal(herramientasDTOEsperado, herramientasDTOActual);
        }
    }
}