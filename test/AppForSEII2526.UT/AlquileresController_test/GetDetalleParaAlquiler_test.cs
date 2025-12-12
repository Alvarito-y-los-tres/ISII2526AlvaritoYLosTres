using AppForSEII2526.API.Controllers;
using AppForSEII2526.API.DTOs;


namespace AppForSEII2526.UT.AlquileresController_test
{
    public class GetDetalleParaAlquiler_test : AppForSEII25264SqliteUT
    {
        public GetDetalleParaAlquiler_test()
        {
            var fabricante = new Fabricante("Ferretería López");
            var herramientas = new List<Herramienta>()
            {
                new Herramienta("Martillo", "Madera", 35, 3, fabricante),
                new Herramienta("Alicates", "Acero", 12, 1, fabricante),
            };

            ApplicationUser usuario = new ApplicationUser("Daniel", "de la Cruz", "642892748", "Daniel@gmail.com");

            var alquiler = new Alquiler("Calle Falsa 123", DateTime.Today, DateTime.Today, 47, TiposMetodosPago.TarjetaCredito, new List<AlquilerItem>(), usuario)
            {
                Id = 1,
                FechaInicio = DateTime.Today,
                FechaFin = DateTime.Today,
                ApplicationUser = usuario
            };

            var itemAlquiler = new AlquilerItem(3, "grande", 35, herramientas[0], alquiler);
            alquiler.AlquilerItems.Add(itemAlquiler);

            _context.Add(fabricante);
            _context.AddRange(herramientas);
            _context.Add(usuario);
            _context.Add(alquiler);
            _context.Add(itemAlquiler);

            _context.SaveChanges();
        }

        [Fact]
        [Trait("Database", "WithoutFixture")]
        [Trait("LevelTesting", "Unit testing")]
        public async Task GetAlquilerDetalle_NotFound_test()
        {
            var mock = new Mock<ILogger<AlquileresController>>();
            ILogger<AlquileresController> logger = mock.Object;
            var controller = new AlquileresController(_context, logger);

            var result = await controller.GetAlquilerDetalle(0);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        [Trait("Database", "WithoutFixture")]
        [Trait("LevelTesting", "Unit testing")]
        public async Task GetAlquilerDetalle_Ok_test()
        {
            var mock = new Mock<ILogger<AlquileresController>>();
            ILogger<AlquileresController> logger = mock.Object;
            var controller = new AlquileresController(_context, logger);

            var expectedAlquiler = new AlquilerDetalleDTO(
                "Daniel",
                "de la Cruz",
                "642892748",
                "daniicruz0005@gmail.com",
                "Calle Falsa 123",
                DateTime.Today,
                47,
                DateTime.Today,
                DateTime.Today,
                1,
                new List<AlquilerItemDTO>
                {
                    new AlquilerItemDTO("Martillo", "Madera", 35, 3)
                }
            );

            var result = await controller.GetAlquilerDetalle(1);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var alquilerActual = Assert.IsType<AlquilerDetalleDTO>(okResult.Value);

            Assert.Equal(expectedAlquiler, alquilerActual);
        }
    }
}