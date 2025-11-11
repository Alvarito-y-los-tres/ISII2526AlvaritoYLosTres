using AppForSEII2526.API.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit; // <--- Asegúrate de tener este
using Moq; // <--- TE FALTA ESTE
using Microsoft.Extensions.Logging;

namespace AppForSEII2526.UT.AlquileresController_test
{
    public class GetDetalleParaAlquiler_test : AppForSEII25264SqliteUT
    {
        public GetDetalleParaAlquiler_test()
        {
            var fabricante = new Fabricante("Ferretería López");
            var herramientas = new List<Herramienta>()
            {
                new Herramienta ("Martillo", "Madera", 35, 3, fabricante),
                new Herramienta ("Alicates", "Acero", 12, 1, fabricante),
            };
            ApplicationUser usuario = new ApplicationUser("Ana", "López", "642892748", "Daniel@gmail.com");
            var alquiler = new Alquiler("Calle Falsa 123", DateTime.Today, 47, TiposMetodosPago.TarjetaCredito, new List<AlquilerItem>(), usuario);

            alquiler.AlquilerItems.Add(new AlquilerItem(3, "grande", 35, herramientas[0], alquiler));
            _context.Add(fabricante);
            _context.AddRange(herramientas);
            _context.Add(alquiler);
            _context.SaveChanges();
        }
        [Fact]
        [Trait("Database", "WithoutFixture")]
        [Trait("LevelTesting", "Unit testing")]
        public async Task GetAlquilerDetalle_NotFound_test()
        {
            //arange
            var mock = new Mock<ILogger<AlquileresController>>();
            ILogger<AlquileresController> logger = mock.Object;
            var controller = new AlquileresController(_context, logger);
            //act
            var result = await controller.GetAlquilerDetalle(0);
            //assert
            Assert.IsType<NotFoundResult>(result);
        }

        // Setup code for the test class can be added here
    }
}
