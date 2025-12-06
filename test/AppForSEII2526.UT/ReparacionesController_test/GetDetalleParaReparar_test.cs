using AppForSEII2526.API.Controllers;
using AppForSEII2526.API.DTOs;

namespace AppForSEII2526.UT.ReparacionesController_test
{
    public class GetDetalleParaReparar_test : AppForSEII25264SqliteUT
    {
        public GetDetalleParaReparar_test()
        {
            // --- Crear fabricantes ---
            var fabricantes = new List<Fabricante>()
            {
                new Fabricante("Ferretería López"),
                new Fabricante("Ferretería García"),
                new Fabricante("Ferretería Ruiz")
            };

            // --- Crear herramientas ---
            var herramientas = new List<Herramienta>()
            {
                new Herramienta("Martillo", "Madera", 5, 3, fabricantes[0]),
                new Herramienta("Taladro", "Acero", 30, 14, fabricantes[1]),
                new Herramienta("Alicates", "Hierro", 4, 5, fabricantes[2])
            };

            // --- Crear usuario ---
            var usuario = new ApplicationUser("Martín", "Álvarez", "660111222", "martin.alvarez@example.com");

            // --- Crear reparación ---
            var reparacion = new Reparacion(usuario, DateTime.Today.AddDays(2), DateTime.Today.AddDays(5));

            // --- Crear items de reparación ---
            var item1 = new ReparacionItem(herramientas[0], reparacion, 2, herramientas[0].Precio, "Mango roto");
            var item2 = new ReparacionItem(herramientas[2], reparacion, 1, herramientas[2].Precio, "Falta muelle interno");

            reparacion.ItemsReparacion.Add(item1);
            reparacion.ItemsReparacion.Add(item2);

            // Calcular precio total 
            decimal precioTotal = 0;

            foreach (var item in reparacion.ItemsReparacion)
            {
                precioTotal += item.Precio * item.Cantidad;
            }

            reparacion.PrecioTotal = precioTotal;

            // --- Guardar en contexto ---
            _context.Fabricantes.AddRange(fabricantes);
            _context.Herramientas.AddRange(herramientas);
            _context.Users.Add(usuario);
            _context.Reparaciones.Add(reparacion);
            _context.SaveChanges();
        }

        [Fact]
        [Trait("Database", "WithoutFixture")]
        [Trait("LevelTesting", "Unit testing")]
        public async Task GetReparacionDetalle_NotFound_test()
        {
            // Arrange
            var mock = new Mock<ILogger<ReparacionesController>>();
            var controller = new ReparacionesController(_context, mock.Object);

            // Act
            var result = await controller.GetReparacionDetalle(999); // ID inexistente

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        [Trait("Database", "WithoutFixture")]
        [Trait("LevelTesting", "Unit testing")]
        public async Task GetReparacionDetalle_Found_test()
        {
            // Arrange
            var mock = new Mock<ILogger<ReparacionesController>>();
            var controller = new ReparacionesController(_context, mock.Object);

            // Expected DTO
            var expected = new ReparacionDetalleDTO(
                "Martín",
                "Álvarez",
                14,
                DateTime.Today.AddDays(2),
                DateTime.Today.AddDays(5),
                new List<ReparacionItemDTO>
                {
                    new ReparacionItemDTO("Martillo","Mango roto",2,5,1),
                    new ReparacionItemDTO("Alicates","Falta muelle interno",1,4,1)

                });

            // Act
            var result = await controller.GetReparacionDetalle(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var dtoActual = Assert.IsType<ReparacionDetalleDTO>(okResult.Value);
            Assert.Equal(expected, dtoActual);
        }
    }
}